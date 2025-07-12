using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class UIMenuSettings
    {
        [Info]
        [SerializeField]
        private string _info;

        public UIProfileSaveMode SaveFileMode = UIProfileSaveMode.Outside;
        public bool SaveOnChange = true;

        [OnValueChanged("SaveFileMode")]
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

    [RequireComponent(typeof(UIMenu))]
    public class UIMenuDataProfileProvider : MonoBehaviour
    {
        public UIMenuSettings Settings = new();

        [Space]
        public string Name = "Menu";
        public UIMenuData Data;

        [Space]
        public UIMenuDataProfile Profile;
        public UIMenuDataProfile Default;

        public Action OnProfileChanged;

        public void OnEnable()
        {
            var menu = GetComponent<UIMenu>();
            if(menu != null )
                Data = menu.Data;

            LoadProfile();
            Profile.OnValueChanged += SaveProfile;
        }

        private static Dictionary<string, UIMenuDataProfile> _profileCache = new();
        public static UIMenuDataProfile GetProfile(string name)
        {
            if (!_profileCache.TryGetValue(name, out var profile))
                return null;

            return profile;
        }

        public void SetProfile(UIMenuDataProfile profile)
        {
            if (profile == null)
                return;

            Profile.OnValueChanged -= SaveProfile;
            Profile = profile;
            Profile.OnValueChanged += SaveProfile;

            _profileCache[Name] = profile;
            OnProfileChanged?.Invoke();
        }

        public void ResetProfile() =>
            Profile?.Data.CopyFrom(Default.Data);

        public UIMenuDataProfile LoadProfile()
        {
            Default ??= CreateProfileInstance("Default");
            Profile ??= CreateProfileInstance("Profile");

            foreach (var data in Data.GetDataItems())
                if (data is UIMenuGeneratorTypeTemplate dataTemplate)
                    Default.AddData(dataTemplate.Reference, dataTemplate.GetDefault());

            if (Settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var parentDirectory = Settings.SaveFileMode == UIProfileSaveMode.Outside;
                var serializedData = (SerializedDictionary<string, object>)default;
                UIMenuDataProfileSerializer.DeserializeData(out serializedData, Name, parentDirectory);
                Profile.Data.CopyFrom(serializedData);
                Profile.Data.AddFrom(Default.Data);
                Profile.name = Name + " Serialized";
            }

            Profile ??= Default;
            OnProfileChanged?.Invoke();
            return Profile;
        }

        public void SaveProfile()
        {
            if (Settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var fileName = Name;
                var parentDirectory = Settings.SaveFileMode == UIProfileSaveMode.Outside;

                UIMenuDataProfileSerializer.SerializeData(Profile.Data, fileName, parentDirectory);
            }
        }

        private UIMenuDataProfile CreateProfileInstance(string name = "Profile")
        {
            var profile = ScriptableObject.CreateInstance<UIMenuDataProfile>();
            profile.name = name + " AutoCreated";
            return profile;
        }
    }
}
