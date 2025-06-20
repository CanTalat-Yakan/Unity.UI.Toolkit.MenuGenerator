using System;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class UIMenuSettings
    {
        [Info]
        [SerializeField]
        private string _info = "This component manages and automatically creates a save file " +
            "and is stored outside the Asset folder, within the Resources directory.\n\n" +
            "Make sure to include the Resources directory and its contents in your Build folder!";

        public string SaveFileName = "UIMenuSettings";
    }

    public enum UIMenuType
    {
        Hierarchical,
        Tabbed,
    }

    public class UIMenu : MonoBehaviour
    {
        [SerializeField] private UIMenuSettings _settings;

        [Space]
        [SerializeField] private UIMenuType _type;
        [SerializeField] private UIMenuData _data;

        public void Start()
        {
            if (_data == null)
                return;

            GetComponent<UIMenuGenerator>().PopulateHierarchy(true, _data.Name, _data.Root);
        }
    }
}
