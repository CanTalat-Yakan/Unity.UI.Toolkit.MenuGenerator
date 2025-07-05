using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuDataProfileProvider : MonoBehaviour
    {
        public UIMenu Menu;
        [OnValueChanged("_menu")] public void OnMenuValueChanged() => SetProfile(Menu?.Profile);

        [Space]
        public UIMenuDataProfile Profile;

        public void Start()
        {
           if (Menu == null)
                return;

           SetProfile(Menu.Profile);
           Menu.OnProfileChanged += SetProfile;
        }

        public Action OnMenuChanged;
        public void SetMenu(UIMenu menu)
        {
            if(menu == null)
                return;

            Menu.OnProfileChanged -= SetProfile;
            Menu = menu;
            Menu.OnProfileChanged += SetProfile;
            OnMenuChanged?.Invoke();
        }

        private static Dictionary<string, UIMenuDataProfile> profileCache = new();
        public static UIMenuDataProfile GetProfile(string name)
        {
            if (!profileCache.TryGetValue(name, out var profile))
                return null;

            return profile;
        }

        public static Action OnProfileChanged;
        public void SetProfile(UIMenuDataProfile profile)
        {
            if(profile == null)
                return;

            Profile = profile;
            profileCache[Menu.name] = profile;
            OnProfileChanged?.Invoke();
        }

        public void ResetProfile() =>
            Profile?.CopyFrom(Menu?.DefaultProfile);
    }
}
