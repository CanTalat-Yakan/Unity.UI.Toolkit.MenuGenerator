using System;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuDataProfileProvider : MonoBehaviour
    {
        public UIMenu Menu;
        [OnValueChanged("_menu")] public void OnMenuValueChanged() => SetProfile(Menu?.Profile);

        [Space]
        public UIMenuDataProfile Profile;

        public void Awake() =>
           Menu.OnProfileChanged += SetProfile;

        public Action OnMenuChanged;
        public void SetMenu(UIMenu menu)
        {
            Menu.OnProfileChanged -= SetProfile;
            Menu = menu;
            Menu.OnProfileChanged += SetProfile;
            OnMenuChanged?.Invoke();
        }

        public void SetProfile(UIMenuDataProfile profile) =>
            Profile = profile;

        public void ResetProfile() =>
            Profile.CopyFrom(Menu?.DefaultProfile);
    }
}
