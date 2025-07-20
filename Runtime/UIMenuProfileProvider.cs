using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class UIMenuProfileProviderSettings
    {
        [Info]
        [SerializeField]
        private string _info =
            "A save file will be created outside the Assets/Build_Data folder, " +
            "within the automatically created directory called Resources.";

        public UIProfileSaveMode SaveFileMode = UIProfileSaveMode.Outside;
        public bool SaveOnChange = true;

        [OnValueChanged(nameof(SaveFileMode))]
        public void OnSaveFileModeValueChanged()
        {
            switch (SaveFileMode)
            {
                case UIProfileSaveMode.None:
                    _info =
                        "No save file will be created. " +
                        "The profile will not persist between sessions and will not be saved to disk.";
                    break;
                case UIProfileSaveMode.Outside:
                    _info =
                        "A save file will be created outside the Assets/Build_Data folder, " +
                        "within the automatically created directory called Resources.";
                    break;
                case UIProfileSaveMode.Inside:
                    _info =
                        "A save file will be created inside the Assets/Build_Data folder, " +
                        "within the automatically created directory called Resources.";
                    break;
                default:
                    break;
            }
        }
    }

    public class UIMenuProfileProvider : MonoBehaviour
    {
        public static Dictionary<string, UIMenuProfile> RegisteredProfiles { get; private set; } = new();

        public UIMenuProfileProviderSettings Settings = new();

        [Space]
        [Info]
        [SerializeField]
        private string _info =
        "UIMenuProfileProvider automatically fetches the UIMenuData and manages the loading, saving, and switching of UIMenuProfile.\n" +
        "It handles profile persistence, default values, and runtime changes, " +
        "allowing menu data to be saved and restored across sessions.";

        [Space]
        public string Name = "Menu";

        [Header("[Automatically Provided]")]
        public UIMenuData Data;

        [OnValueChanged(nameof(Name))]
        public void OnNameValueChanged()
        {
            var menu = GetComponent<UIMenu>();
            if (menu != null)
                menu.Name = Name;
        }

        [Space]
        public UIMenuProfile Profile;
        public UIMenuProfile Default;

        public Action OnProfileChanged;

        public void OnEnable()
        {
            var menu = GetComponent<UIMenu>();
            if (menu != null)
                Data = menu.Data;

            LoadProfile();
            Profile.OnValueChanged += (_) => SaveProfile();
        }

        public static bool TryGetProfile(string name, out UIMenuProfile outputProfile) =>
            RegisteredProfiles.TryGetValue(name, out outputProfile);

        public void SetProfile(UIMenuProfile profile)
        {
            if (profile == null)
                return;

            Profile.OnValueChanged -= (_) => SaveProfile();
            Profile = profile;
            Profile.OnValueChanged += (_) => SaveProfile();

            RegisteredProfiles[Name] = profile;
            OnProfileChanged?.Invoke();
        }

        public void ResetProfile() =>
            Profile?.Data.CopyFrom(Default.Data);

        public UIMenuProfile LoadProfile()
        {
            Default ??= CreateProfileInstance("Default");
            Profile ??= CreateProfileInstance("Profile");

            foreach (var data in Data.EnumerateAllData())
                if (data is UIMenuTypeDataBase dataTemplate)
                    Default.Add(dataTemplate.Reference, dataTemplate.GetDefault());

            if (Settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var parentDirectory = Settings.SaveFileMode == UIProfileSaveMode.Outside;
                var serializedData = (SerializedDictionary<string, object>)default;
                UIMenuProfileSerializer.DeserializeData(out serializedData, Name, parentDirectory);
                Profile.Data.CopyFrom(serializedData);
                Profile.Data.AddFrom(Default.Data);
                Profile.name = Name + " Serialized";
            }

            Profile ??= Default;
            SetProfile(Profile);
            return Profile;
        }

        public void SaveProfile()
        {
            if (Settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var fileName = Name;
                var parentDirectory = Settings.SaveFileMode == UIProfileSaveMode.Outside;

                UIMenuProfileSerializer.SerializeData(Profile.Data, fileName, parentDirectory);
            }
        }

        private UIMenuProfile CreateProfileInstance(string name = "Profile")
        {
            var profile = ScriptableObject.CreateInstance<UIMenuProfile>();
            profile.name = name + " AutoCreated";
            return profile;
        }
    }
}
