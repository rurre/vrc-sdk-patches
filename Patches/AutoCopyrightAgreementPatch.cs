using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using VRC.SDKBase;
using VRC.SDKBase.Editor;
using static Pumkin.VrcSdkPatches.PumkinPatcherLogger;
using static Pumkin.VrcSdkPatches.ReflectionHelpers;

namespace Pumkin.VrcSdkPatches
{
    internal static class AutoCopyrightAgreementPatch
    {
        const string SkippedDialogMessage = "Skipped VRC copyright agreement dialog because you already agreed to it.";
        const string TargetMethodFullName = "VRC.SDKBase.VRCCopyrightAgreement:CheckCopyrightAgreement";

        static readonly MethodInfo transpilerMethod = AccessTools.Method(typeof(AutoCopyrightAgreementPatch), nameof(Transpiler));
        static readonly MethodInfo targetMethod = GetMethodIncludingAsync(TargetMethodFullName);

        static readonly MethodInfo loggerMethod = AccessTools.Method(typeof(PumkinPatcherLogger), nameof(PumkinPatcherLogger.Log));
        
        public static void Patch(Harmony harmony)
        {
            Log($"Patching <b>{nameof(VRCCopyrightAgreement)}:{nameof(VRCCopyrightAgreement.CheckCopyrightAgreement)}</b> to automatically accept content agreement dialog. You own all this content, right?");
            harmony.Patch(targetMethod, transpiler: new HarmonyMethod(transpilerMethod));
        }

        public static void UnPatch(Harmony harmony)
        {
            Log($"UnPatching <b>{nameof(VRCCopyrightAgreement)}:{nameof(VRCCopyrightAgreement.CheckCopyrightAgreement)}</b> to no longer automatically accept content agreement dialog.");
            harmony.Unpatch(targetMethod, transpilerMethod);
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var taskCompletionSourceConstructor = AccessTools.Constructor(typeof(TaskCompletionSource<bool>));
            var ownershipExceptionConstructor = AccessTools.Constructor(typeof(OwnershipException), new []{ typeof(string) });

            int beginRemoveIndex = -1;
            int endRemoveIndex = -1;

            // Find the new TaskCmpletionSource<bool>() constructor call
            for(int i = 0; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Newobj &&
                   codes[i].operand.Equals(taskCompletionSourceConstructor) &&
                   codes.TryGet(i - 1, out var codeMinus1) && codeMinus1.opcode == OpCodes.Ldfld && 
                   codeMinus1.operand.ToString().Contains("VRC.SDKBase.VRCCopyrightAgreement+<>c__DisplayClass") &&
                   codes.TryGet(i - 2, out var codeMinus2) && codeMinus2.opcode == OpCodes.Ldarg_0)
                {
                    beginRemoveIndex = i - 2;
                    break;
                }
            }
            
            if(beginRemoveIndex == -1)
                throw new PatchFailException("Can't find index of beginning of code to skip.");

            // Find the throw copyright ownership exception code
            for(int i = 0; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Ldstr &&
                   codes.TryGet(i + 1, out var codePlus1) &&
                   codePlus1.opcode == OpCodes.Newobj && codePlus1.operand.Equals(ownershipExceptionConstructor) &&
                   codes.TryGet(i + 2, out var codePlus2) &&
                   codePlus2.opcode == OpCodes.Throw)
                {
                    endRemoveIndex = i + 2;
                    break;
                }
            }
            
            if(endRemoveIndex == -1)
                throw new PatchFailException("Can't find index of end of code to skip.");

            for(int i = beginRemoveIndex; i <= endRemoveIndex; i++)
            {
                codes[i].opcode = OpCodes.Nop;
                codes[i].operand = null;
            }
            
            // Log to console that we're skipping the copyright dialog
            codes[endRemoveIndex - 1].opcode = OpCodes.Ldstr;
            codes[endRemoveIndex - 1].operand = SkippedDialogMessage;
            
            codes[endRemoveIndex].opcode = OpCodes.Call;
            codes[endRemoveIndex].operand = loggerMethod;
            
            return codes;
        }
    }
}

    