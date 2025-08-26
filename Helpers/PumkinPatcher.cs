using HarmonyLib;
using UnityEditor;
using VRC.SDKBase.Editor.Api;
using static Pumkin.VrcSdkPatches.PumkinPatcherLogger;
using static Pumkin.VrcSdkPatches.ReflectionHelpers;

namespace Pumkin.VrcSdkPatches
{
    internal static class PumkinPatcher
    {
        const string HarmonyId = "Pumkin.VrcSdkPatches";
        
        [InitializeOnLoadMethod]
        static void ApplyPatches()
        {
            var harmony = new Harmony(HarmonyId);
            
            AvatarThumbnailNamePatch.Patch(harmony);
            AutoCopyrightAgreementPatch.Patch(harmony);
            
            AssemblyReloadEvents.beforeAssemblyReload += () => { harmony.UnpatchAll(HarmonyId); };
        }
    }
}