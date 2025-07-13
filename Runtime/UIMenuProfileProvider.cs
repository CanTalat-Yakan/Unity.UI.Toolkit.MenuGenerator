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
    public class UIMenuProfileProvider : MonoBehaviour
    {
        public UIMenuSettings Settings = new();

        [Space]
        public string Name = "Menu";
        public UIMenuData Data;

        [OnValueChanged("Name")]
        public void OnNameValueChanged() => GetComponent<UIMenu>().Name = Name;

        [Space]
        public UIMenuProfile Profile;
        public UIMenuProfile Default;

        public Action OnProfileChanged;

        public void OnEnable()
        {
            var menu = GetComponent<UIMenu>();
            if(menu != null )
                Data = menu.Data;

            LoadProfile();
            Profile.OnValueChanged += SaveProfile;
        }

        private static Dictionary<string, UIMenuProfile> _profileCache = new();
        public static UIMenuProfile GetProfile(string name)
        {
            if (!_profileCache.TryGetValue(name, out var profile))
                return null;

            return profile;
        }

        public void SetProfile(UIMenuProfile profile)
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

        public UIMenuProfile LoadProfile()
        {
            Default ??= CreateProfileInstance("Default");
            Profile ??= CreateProfileInstance("Profile");

            foreach (var data in Data.GetDataItems())
                if (data is UIMenuTypeDataBase dataTemplate)
                    Default.AddData(dataTemplate.Reference, dataTemplate.GetDefault());

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
            OnProfileChanged?.Invoke();
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
