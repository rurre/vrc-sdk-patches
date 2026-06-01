using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Pumkin.VrcSdkPatches
{
    [Serializable]
    internal static class PumkinPatcherSettings
    {
        public static bool AnonymizeAvatarThumbnailNames
        {
            get => _anonymizeAvatarThumbnailNames;
            set
            {
                _anonymizeAvatarThumbnailNames = value;
                PumkinPatcher.SetAvatarThumbnailPatchState(value);
            }
        }
        static bool _anonymizeAvatarThumbnailNames;
        
        public static List<string> ReplacementNames
        {
            get => _replacementNames;
            set => _replacementNames = value;
        }
        static List<string> _replacementNames = new List<string>();

        public static bool AutoAcceptCopyrightDialog
        {
            get => _autoAcceptCopyrightDialog;
            set
            {
                _autoAcceptCopyrightDialog = value;
                PumkinPatcher.SetAutoAcceptCopyrightDialogPatchState(value);
                _lastAgreedToAgreedToAgreementVersion = LatestAgreementVersion;
            }
        }
        static bool _autoAcceptCopyrightDialog;

        const int LatestAgreementVersion = 2;
        
        static int LastAgreedToAgreementVersion
        {
            get => _lastAgreedToAgreedToAgreementVersion;
            set => _lastAgreedToAgreedToAgreementVersion = value;
        }
        static int _lastAgreedToAgreedToAgreementVersion;

        class ArrayWrapper { public string[] array = Array.Empty<string>(); }

        public static void LoadSettings()
        {
            _anonymizeAvatarThumbnailNames = EditorPrefs.GetBool($"{Application.productName}:{nameof(AnonymizeAvatarThumbnailNames)}", false);
            
            string replacementJson = EditorPrefs.GetString($"{Application.productName}:{nameof(ReplacementNames)}", "{\"array\":[]}");
            var arrayWrapper = new ArrayWrapper();
            JsonUtility.FromJsonOverwrite(replacementJson, arrayWrapper);
            _replacementNames = arrayWrapper.array?.Length > 0 ? arrayWrapper.array?.ToList() : new List<string>();
            
            _lastAgreedToAgreedToAgreementVersion = EditorPrefs.GetInt($"{Application.productName}:{nameof(LastAgreedToAgreementVersion)}", 0);
            if(EditorPrefs.HasKey($"{Application.productName}:{nameof(AutoAcceptCopyrightDialog)}"))
            {
                _autoAcceptCopyrightDialog = EditorPrefs.GetBool($"{Application.productName}:{nameof(AutoAcceptCopyrightDialog)}", false);
                if(_autoAcceptCopyrightDialog && _lastAgreedToAgreedToAgreementVersion < LatestAgreementVersion)
                {
                    _autoAcceptCopyrightDialog = false;
                    EditorPrefs.DeleteKey($"{Application.productName}:{nameof(AutoAcceptCopyrightDialog)}");
                    EditorUtility.DisplayDialog("Pumkin's VRC SDK Patches: Auto Accept Copyright Dialog",
                        "The copyright agreement for the Auto Accept Copyright Dialog patch has changed.\n\nThe patch has been disabled.\n\nPlease re-enable it and agree to the new agreement to continue using it.",
                        "OK");
                }
            }
        }
        
        public static void SaveSettings()
        {
            EditorPrefs.SetBool($"{Application.productName}:{nameof(AnonymizeAvatarThumbnailNames)}", _anonymizeAvatarThumbnailNames);
            EditorPrefs.SetBool($"{Application.productName}:{nameof(AutoAcceptCopyrightDialog)}", _autoAcceptCopyrightDialog);
            
            if(AutoAcceptCopyrightDialog)
                EditorPrefs.SetInt($"{Application.productName}:{nameof(LastAgreedToAgreementVersion)}", _lastAgreedToAgreedToAgreementVersion);
            else if(EditorPrefs.HasKey($"{Application.productName}:{nameof(LastAgreedToAgreementVersion)}"))
                EditorPrefs.DeleteKey($"{Application.productName}:{nameof(LastAgreedToAgreementVersion)}");

            // Sanitize names again just in case and remove nulls
            var sanitizedNonNullNames = _replacementNames
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(StringSanitizer.RemoveInvalidFilenameChars)
                .ToArray();
            
            string replacementsJson = JsonUtility.ToJson(new ArrayWrapper { array = sanitizedNonNullNames });
            EditorPrefs.SetString($"{Application.productName}:{nameof(ReplacementNames)}", replacementsJson);
        }
    }
}