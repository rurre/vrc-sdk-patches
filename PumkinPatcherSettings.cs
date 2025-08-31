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
        static List<string> _replacementNames;

        class ArrayWrapper { public string[] array = Array.Empty<string>(); }

        public static void LoadSettings()
        {
            _anonymizeAvatarThumbnailNames = EditorPrefs.GetBool($"{Application.productName}:{nameof(AnonymizeAvatarThumbnailNames)}", false);
            
            string replacementJson = EditorPrefs.GetString($"{Application.productName}:{nameof(ReplacementNames)}", "{\"array\":[]}");
            var arrayWrapper = new ArrayWrapper();
            JsonUtility.FromJsonOverwrite(replacementJson, arrayWrapper);
            _replacementNames = arrayWrapper?.array?.Length > 0 ? arrayWrapper?.array?.ToList() : new List<string>();
        }
        
        public static void SaveSettings()
        {
            EditorPrefs.SetBool($"{Application.productName}:{nameof(AnonymizeAvatarThumbnailNames)}", _anonymizeAvatarThumbnailNames);
            
            string replacementsJson = JsonUtility.ToJson(new ArrayWrapper() { array = _replacementNames.ToArray() });
            EditorPrefs.SetString($"{Application.productName}:{nameof(ReplacementNames)}", replacementsJson);
        }
    }
}