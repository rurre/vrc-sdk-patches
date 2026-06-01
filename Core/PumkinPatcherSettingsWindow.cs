using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using VRC.SDKBase;
using static Pumkin.VrcSdkPatches.PumkinPatcherLogger;

namespace Pumkin.VrcSdkPatches
{
    [Serializable]
    public class PumkinPatcherSettingsWindow : EditorWindow
    {
        const string PackageJsonGuid = "8021f47c8e48e414c8afc59ee31cc8c5";
        readonly string CopyrightDialogAgreementText = "By enabling this option I agree that Pumkin - the creator of this patch, cannot be held accountable for any of the content I upload to VRChat in any way.\n\nIn addition, I authorise this patch to sign the following VRChat agreement on my behalf, for every future upload while it is active:\n\n" + VRCCopyrightAgreement.AgreementText;
        
        [Serializable] class VersionWrapper { public string version; }

        [MenuItem("Tools/Pumkin/VRC SDK Patches")]
        public static void ShowWindow()
        {
            var window = GetWindow<PumkinPatcherSettingsWindow>("Patcher Settings");
            window.Show();
        }
        
        void OnDestroy()
        {
            EditorApplication.quitting -= SaveSettings;
            AssemblyReloadEvents.beforeAssemblyReload -= SaveSettings;
            SaveSettings();
        }


        Toggle copyrightDialog;

        void CreateGUI()
        {
            EditorApplication.quitting -= SaveSettings;
            EditorApplication.quitting += SaveSettings;
            AssemblyReloadEvents.beforeAssemblyReload -= SaveSettings;
            AssemblyReloadEvents.beforeAssemblyReload += SaveSettings;
            
            PumkinPatcherSettings.LoadSettings();
            
            var tree = Resources.Load<VisualTreeAsset>("Pumkin/UI/PumkinPatcherSettingsWindow");
            tree.CloneTree(rootVisualElement);
            
            rootVisualElement.Q<Label>("version").text = $"v{GetPackageVersion()}";

            copyrightDialog = rootVisualElement.Q<Toggle>("autoAcceptCopyrightDialog");
            copyrightDialog.SetValueWithoutNotify(PumkinPatcherSettings.AutoAcceptCopyrightDialog);
            copyrightDialog.RegisterValueChangedCallback(HandleCopyrightDialogSetting);
            
            var anonymizeNames = rootVisualElement.Q<Toggle>("anonymizeAvatarThumbnailNames");
            anonymizeNames.SetValueWithoutNotify(PumkinPatcherSettings.AnonymizeAvatarThumbnailNames);
            anonymizeNames.RegisterValueChangedCallback(evt => PumkinPatcherSettings.AnonymizeAvatarThumbnailNames = evt.newValue);
            
            var names = rootVisualElement.Q<ListView>("replacementNames");
            names.itemsSource = PumkinPatcherSettings.ReplacementNames;
            names.makeItem = () => new TextField() { isDelayed = true };

            names.bindItem = (ve, index) =>
            {
                var textField = ve as TextField;
                textField.SetValueWithoutNotify(PumkinPatcherSettings.ReplacementNames[index]);
                textField.RegisterValueChangedCallback(evt =>
                {
                    PumkinPatcherSettings.ReplacementNames[index] = StringSanitizer.RemoveInvalidFilenameChars(evt.newValue);
                });
            };
            names.unbindItem = (ve, index) =>
            {
                ((TextField)ve).UnregisterValueChangedCallback(evt => PumkinPatcherSettings.ReplacementNames[index] = StringSanitizer.RemoveInvalidFilenameChars(evt.newValue));
            };
        }

        void HandleCopyrightDialogSetting(ChangeEvent<bool> evt)
        {
            if(evt.newValue)
            {
                if(EditorUtility.DisplayDialog("Copyright ownership agreement", CopyrightDialogAgreementText, "OK", "No"))
                    PumkinPatcherSettings.AutoAcceptCopyrightDialog = true;
                else
                    copyrightDialog.SetValueWithoutNotify(false);
            }
            else
                PumkinPatcherSettings.AutoAcceptCopyrightDialog = false;
        }

        void SaveSettings()
        {
            PumkinPatcherSettings.SaveSettings();
        }

        Version GetPackageVersion()
        {
            string path = AssetDatabase.GUIDToAssetPath(PackageJsonGuid);
            if(string.IsNullOrEmpty(path))
            {
                LogError("Couldn't find package.json. Did the guid change? Ask Pumkin!");
                return new Version(0, 0, 0);
            }
                
            var text = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            if(text == null || string.IsNullOrWhiteSpace(text.text))
            {
                LogError("Couldn't load package.json for version. Are the contents valid json?");
                return new Version(0, 0, 0);
            }

            VersionWrapper wrapper = new VersionWrapper();
            JsonUtility.FromJsonOverwrite(text.text, wrapper);
            
            if(wrapper == null || string.IsNullOrWhiteSpace(wrapper.version))
            {
                LogError("Couldn't get version form package.json. What the heck happened here?");
                return new Version(0, 0, 0);
            }
            return new Version(wrapper.version);
        }
    }
}
