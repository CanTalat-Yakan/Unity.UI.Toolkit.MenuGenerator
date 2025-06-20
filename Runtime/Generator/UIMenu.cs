using System;
using UnityEngine;

namespace UnityEssentials
{
    public enum UIProfileSaveMode
    {
        None,
        Outside,
        Inside,
    }

    public enum UIMenuType
    {
        Hierarchical,
        Tabbed,
    }

    [Serializable]
    public class UIMenuSettings
    {
        [Info]
        [SerializeField]
        private string _info = "This component manages and automatically creates a save file " +
            "and is stored outside the Asset folder, within the Resources directory.\n\n" +
            "Make sure to include the Resources directory and its contents in your Build folder!";

        public UIProfileSaveMode SaveFileMode = UIProfileSaveMode.None;
        public string SaveFileName = "UIMenuSettings";
    }

    public class UIMenu : MonoBehaviour
    {
        [SerializeField] private UIMenuSettings _settings;

        [Space]
        [SerializeField] private UIMenuType _type;
        [SerializeField] private UIMenuData _data;

        [HideInInspector] public UIMenuGenerator Generator => _generator ??= GetComponent<UIMenuGenerator>();
        private UIMenuGenerator _generator;

        public void Start()
        {
            if (_data == null)
                return;

            Generator.PopulateHierarchy(true, _data.Name, _data.Root);
        }

        [OnValueChanged("_type")]
        public void OnTypeValueChanged()
        {
            DestroyAllChildren();

            var go = Instantiate(_type switch
            {
                UIMenuType.Hierarchical => Generator.Data.HierarchicalMenuTemplate,
                UIMenuType.Tabbed => Generator.Data.TabbedMenuTemplate,
                _ => null
            }, parent: transform);
            go.name = _type.ToString() + " Menu UI Document";

            Generator.Initialize();
            Generator.Fetch();
        }

        private void DestroyAllChildren()
        {
            while (transform.childCount > 0)
                if (Application.isEditor)
                    DestroyImmediate(transform.GetChild(0).gameObject);
                else Destroy(transform.GetChild(0).gameObject);
        }
    }
}
