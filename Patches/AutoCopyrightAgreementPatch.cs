using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEditor;
using static Pumkin.VrcSdkPatches.PumkinPatcherLogger;
using static Pumkin.VrcSdkPatches.ReflectionHelpers;

namespace Pumkin.VrcSdkPatches
{
    internal static class AutoCopyrightAgreementPatch
    {
        const string TargetMethod = "VRC.SDKBase.VRCCopyrightAgreement:CheckCopyrightAgreement";

        public static void Patch(Harmony harmony)
        {
            Log($"Patching <b>{TargetMethod}</b> to automatically accept content agreement dialog. You own all this content, right?");

            var checkAgreementMethod = GetMethodIncludingAsync(TargetMethod);
            MethodInfo thisTranspiler = AccessTools.Method(typeof(AutoCopyrightAgreementPatch), nameof(Transpiler));

            harmony.Patch(checkAgreementMethod, transpiler: new HarmonyMethod(thisTranspiler));
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var taskCompletionSourceConstructor = AccessTools.Constructor(typeof(TaskCompletionSource<bool>));
            var setResultMethod = AccessTools.Method(typeof(TaskCompletionSource<bool>), nameof(TaskCompletionSource<bool>.SetResult));
            var taskProperty = AccessTools.Property(typeof(TaskCompletionSource<bool>), "Task").GetGetMethod();

            bool foundTaskCreation = false;
            bool foundTaskAwait = false;
            int insertIndex = -1;
            int removeStartIndex = -1;
            int removeEndIndex = -1;

            // Find the TaskCompletionSource constructor call
            for(int i = 0; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Newobj &&
                   codes[i].operand.Equals(taskCompletionSourceConstructor))
                {
                    foundTaskCreation = true;
                    insertIndex = i + 2; // After the constructor and store
                    removeStartIndex = i + 2;
                    break;
                }
            }

            if(!foundTaskCreation)
            {
                LogError("Failed. Can't find task creation line.");
                return codes;
            }

            // Find the "int task = (int) await agreementTask.Task;" line by looking for GetAwaiter pattern
            var getAwaiterMethod = AccessTools.Method(typeof(Task<bool>), "GetAwaiter");

            for(int i = insertIndex; i < codes.Count; i++)
            {
                if(codes[i].opcode == OpCodes.Callvirt &&
                   codes[i].operand.Equals(taskProperty))
                {
                    // Look for GetAwaiter call after get_Task
                    if(i + 1 < codes.Count &&
                       codes[i + 1].opcode == OpCodes.Callvirt &&
                       codes[i + 1].operand.Equals(getAwaiterMethod))
                    {
                        foundTaskAwait = true;
                        removeEndIndex = i + 3; // Include the store operation after GetAwaiter
                        break;
                    }
                }
            }

            if(!foundTaskAwait)
            {
                LogError("Failed. Can't find task await line.");
                return codes;
            }

            // Remove the code between TaskCompletionSource creation and task await
            for(int i = removeEndIndex - 1; i >= removeStartIndex; i--)
            {
                codes.RemoveAt(i);
            }

            // Insert SetResult(true) call at the insertion point
            var newInstructions = new List<CodeInstruction>
            {
                // Load the TaskCompletionSource (should be on stack or in local variable)
                new CodeInstruction(OpCodes.Ldloc_S, GetTaskCompletionSourceLocalIndex(codes, insertIndex)),
                new CodeInstruction(OpCodes.Ldc_I4_1), // Load true (1)
                new CodeInstruction(OpCodes.Call, setResultMethod) // Call SetResult(true)
            };

            codes.InsertRange(removeStartIndex, newInstructions);

            Log("Success!");
            return codes;
        }

        static int GetTaskCompletionSourceLocalIndex(List<CodeInstruction> codes, int searchStart)
        {
            // Look backwards from searchStart to find the store operation
            for(int i = searchStart - 1; i >= 0; i--)
            {
                if(codes[i].opcode == OpCodes.Stloc ||
                   codes[i].opcode == OpCodes.Stloc_S ||
                   codes[i].opcode == OpCodes.Stloc_0 ||
                   codes[i].opcode == OpCodes.Stloc_1 ||
                   codes[i].opcode == OpCodes.Stloc_2 ||
                   codes[i].opcode == OpCodes.Stloc_3)
                {
                    if(codes[i].opcode == OpCodes.Stloc_0) return 0;
                    if(codes[i].opcode == OpCodes.Stloc_1) return 1;
                    if(codes[i].opcode == OpCodes.Stloc_2) return 2;
                    if(codes[i].opcode == OpCodes.Stloc_3) return 3;
                    if(codes[i].operand is int index) return index;
                    if(codes[i].operand is LocalBuilder lb) return lb.LocalIndex;
                }
            }
            return 0;
        }
    }
}

    