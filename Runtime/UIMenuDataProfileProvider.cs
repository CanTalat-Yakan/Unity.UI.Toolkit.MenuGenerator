using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    [RequireComponent(typeof(UIMenu))]
    public class UIMenuDataProfileProvider : MonoBehaviour
    {
        public UIMenuDataProfile Profile;
        public UIMenuDataProfile Default;

        public Action OnProfileChanged;

        [HideInInspector] public UIMenu Menu => _menu ??= GetComponent<UIMenu>();
        private UIMenu _menu;

        public void OnEnable()
        {
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

            _profileCache[Menu.name] = profile;
            OnProfileChanged?.Invoke();
        }

        public void ResetProfile() =>
            Profile?.Data.CopyFrom(Menu?.DefaultProfile.Data);

        public UIMenuDataProfile LoadProfile()
        {
            Default ??= CreateProfileInstance("Default");
            Profile ??= CreateProfileInstance("Profile");

            foreach (var data in Menu.Data.GetDataItems())
                if (data is UIMenuGeneratorTypeTemplate dataTemplate)
                    Default.AddData(dataTemplate.Reference, dataTemplate.GetDefault());

            if (Menu.Settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var fileName = Menu.Name;
                var parentDirectory = Menu.Settings.SaveFileMode == UIProfileSaveMode.Outside;

                var serializedData = (SerializedDictionary<string, object>)default;
                UIMenuDataProfileSerializer.DeserializeData(out serializedData, fileName, parentDirectory);
                Profile.Data.CopyFrom(serializedData);
                Profile.Data.AddFrom(Default.Data);
                Profile.name = fileName + " Serialized";
            }

            Profile ??= Default;
            OnProfileChanged?.Invoke();
            return Profile;
        }

        public void SaveProfile()
        {
            if (Menu.Settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var fileName = Menu.Name;
                var parentDirectory = Menu.Settings.SaveFileMode == UIProfileSaveMode.Outside;

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
