using System;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class UIMenuSettings
    {
        public string SaveFileName = "UIMenuSettings";
    }

    public enum UIMenuType
    {
        Hierarchical, 
        Tabbed
    }

    public class UIMenu : MonoBehaviour
    {
        [Info] 
        [SerializeField] private string _info = 
            "UIMenu is a component that manages the UI menu system. " +
            "It can be used to create hierarchical or tabbed menus based on the provided settings and generator data.";

        [SerializeField] private UIMenuSettings _settings;

        [Space]
        [SerializeField] private UIMenuType _menuType;
        [SerializeField] private UIMenuGeneratorData _menuGeneratorData;
    }
}
