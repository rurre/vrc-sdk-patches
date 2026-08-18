using HarmonyLib;
using UnityEditor;

namespace Pumkin.VrcSdkPatches
{
    internal static class PumkinPatcher
    {
        const string HarmonyId = "Pumkin.VrcSdkPatches";

        static Harmony Harmony
        {
            get
            {
                if(_harmony == null)
                    _harmony = new Harmony(HarmonyId);
                return _harmony;
            }
        }
        static Harmony _harmony;
        
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            PumkinPatcherSettings.LoadSettings();
            SetAvatarThumbnailPatchState(PumkinPatcherSettings.AnonymizeAvatarThumbnailNames);
            SetAutoAcceptCopyrightDialogPatchState(PumkinPatcherSettings.AutoAcceptCopyrightDialog);
            SetAddOpenTestAvatarButtonState(PumkinPatcherSettings.AddOpenTestAvatarButton);
            
            AssemblyReloadEvents.beforeAssemblyReload += () => { Harmony.UnpatchAll(HarmonyId); };
        }

        // TODO: Make these non static classes and stop copy pasting code
        internal static void SetAvatarThumbnailPatchState(bool enabled)
        {
            if(enabled)
                AvatarThumbnailNamePatch.Patch(Harmony);
            else
                AvatarThumbnailNamePatch.UnPatch(Harmony);
        }

        internal static void SetAutoAcceptCopyrightDialogPatchState(bool enabled)
        {
            if(enabled)
                AutoCopyrightAgreementPatch.Patch(Harmony);
            else
                AutoCopyrightAgreementPatch.UnPatch(Harmony);
        }

        internal static void SetAddOpenTestAvatarButtonState(bool enabled)
        {
            if(enabled)
                AddOpenTestAvatarButtonPatch.Patch(Harmony);
            else
                AddOpenTestAvatarButtonPatch.UnPatch(Harmony);
        }
    }
}