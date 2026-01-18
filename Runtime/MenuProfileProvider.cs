using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class MenuProfileProviderSettings
    {
        [Info]
        [SerializeField]
        private string _info =
            "A save file will be created outside the Assets/Build_Data folder, " +
            "within the automatically created directory called Resources.";

        public MenuProfileSaveMode SaveFileMode = MenuProfileSaveMode.Outside;
        public bool SaveOnChange = true;

        [OnValueChanged(nameof(SaveFileMode))]
        public void OnSaveFileModeValueChanged()
        {
            switch (SaveFileMode)
            {
                case MenuProfileSaveMode.None:
                    _info =
                        "No save file will be created. " +
                        "The profile will not persist between sessions and will not be saved to disk.";
                    break;
                case MenuProfileSaveMode.Outside:
                    _info =
                        "A save file will be created outside the Assets/Build_Data folder, " +
                        "within the automatically created directory called Resources.";
                    break;
                case MenuProfileSaveMode.Inside:
                    _info =
                        "A save file will be created inside the Assets/Build_Data folder, " +
                        "within the automatically created directory called Resources.";
                    break;
                default:
                    break;
            }
        }
    }

    [DefaultExecutionOrder(-999)]
    public class UIMenuProfileProvider : MonoBehaviour
    {
        public static Dictionary<string, MenuProfile> RegisteredProfiles { get; private set; } = new();

        public MenuProfileProviderSettings Settings = new();

        [Space]
        [Info]
        [SerializeField]
        private string _info =
        "UIMenuProfileProvider automatically fetches the UIMenuData and manages the loading, saving, and switching of UIMenuProfile.\n" +
        "It handles profile persistence, default values, and runtime changes, " +
        "allowing menu data to be saved and restored across sessions.";

        [Space]
        public string Name = "Menu";
        public MenuData Data;

        [OnValueChanged(nameof(Name))]
        public void OnNameValueChanged()
        {
            var menu = GetComponent<Menu>();
            if (menu != null)
                menu.Name = Name;
        }

        [OnValueChanged(nameof(Data))]
        public void OnDataValueChanged()
        {
            var menu = GetComponent<Menu>();
            if (menu != null)
                menu.Data = Data;
        }

        [HideInInspector] public MenuProfile Profile;
        [HideInInspector] public MenuProfile Default;

        public Action OnProfileChanged;

        public void Awake()
        {
            Data ??= GetComponent<Menu>().Data;
            LoadProfile();
            SaveProfile();

            Profile.OnValueChanged += (_) => SaveProfile();
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod()]
        public static void ClearRegisteredProfiles() =>
            RegisteredProfiles.Clear();

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                ClearRegisteredProfiles();
            }
        }
#endif

        public static bool TryGetProfile(string name, out MenuProfile outputProfile) =>
            RegisteredProfiles.TryGetValue(name, out outputProfile);

        public void SetProfile(MenuProfile profile)
        {
            if (profile == null)
                return;

            if (Settings.SaveOnChange)
                Profile.OnValueChanged -= (_) => SaveProfile();

            Profile = profile;

            if (Settings.SaveOnChange)
                Profile.OnValueChanged += (_) => SaveProfile();

            RegisteredProfiles[Name] = profile;
            OnProfileChanged?.Invoke();
        }

        public void ResetProfile() =>
            Profile?.Data.CopyFrom(Default.Data);

        public MenuProfile LoadProfile()
        {
            Default ??= CreateProfileInstance("Default");
            Profile ??= CreateProfileInstance("Profile");

            if (Data == null)
                return Profile;

            foreach (var data in Data.EnumerateAllData())
                if (data is MenuTypeDataBase dataTemplate)
                    Default.Add(dataTemplate.Reference, dataTemplate.GetDefault());

            if (Settings.SaveFileMode != MenuProfileSaveMode.None)
            {
                var parentDirectory = Settings.SaveFileMode == MenuProfileSaveMode.Outside;
                var serializedData = (SerializedDictionary<string, object>)default;
                MenuProfileSerializer.DeserializeData(out serializedData, Name, parentDirectory);
                Profile.Data.CopyFrom(serializedData);
                Profile.name = Name + " Serialized";
            }

            Profile.Data.AddFrom(Default.Data);
            SetProfile(Profile);
            return Profile;
        }

        public void SaveProfile()
        {
            if (Settings.SaveFileMode != MenuProfileSaveMode.None)
            {
                var fileName = Name;
                var parentDirectory = Settings.SaveFileMode == MenuProfileSaveMode.Outside;

                MenuProfileSerializer.SerializeData(Profile.Data, fileName, parentDirectory);
            }
        }

        private MenuProfile CreateProfileInstance(string name = "Profile")
        {
            var profile = ScriptableObject.CreateInstance<MenuProfile>();
            profile.name = name + " AutoCreated";
            return profile;
        }
    }
}