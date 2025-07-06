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

        [HideInInspector] public UIMenu Menu { get; private set; }

        public void OnEnable()
        {
            Menu = GetComponent<UIMenu>();
            if (Menu.Settings.SaveOnChange)
                OnProfileChanged += SaveProfile;

            LoadProfile();
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

            Profile = profile;
            _profileCache[Menu.name] = profile;
            OnProfileChanged?.Invoke();
        }

        public void ResetProfile() =>
            Profile?.CopyFrom(Menu?.DefaultProfile);

        public UIMenuDataProfile LoadProfile()
        {
            Default ??= CreateProfileInstance("Default");

            foreach (var data in Menu.Data.GetDataItems())
                if (data is UIGeneratorTypeTemplate dataTemplate)
                    dataTemplate.ProfileAddDefault(Default);

            if (Menu.Settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var fileName = Menu.Name;
                var parentDirectory = Menu.Settings.SaveFileMode == UIProfileSaveMode.Outside;

                UIMenuDataProfileSerializer.DeserializeData(out Profile, fileName, parentDirectory);
                Profile.AddFrom(Default);
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

                UIMenuDataProfileSerializer.SerializeData(Profile, fileName, parentDirectory);
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
