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
        public static Action<UIMenuData> ShowUIBuilder { get; set; }

        [SerializeField] private UIMenuSettings _settings = new();

        [Space]
        [SerializeField] private UIMenuType _type;
        [SerializeField] private UIMenuData _data;

        [HideInInspector] public UIMenuGenerator Generator => GetComponent<UIMenuGenerator>();

        public void Start()
        {
            if (_data == null)
                return;

            Generator.PopulateHierarchy(true, _data.Name, _data.Root);
        }

        [Button()]
        public void EditUIBuilder() =>
            ShowUIBuilder?.Invoke(_data);

        [OnValueChanged("_type")]
        public void OnTypeValueChanged()
        {
            DestroyAllChildren();
            InstantiateMenu();

            GetProfile();
            SaveProfile();

            Generator.Initialize();
            Generator.Fetch();
        }

        [ContextMenu("Get Profile")]
        public void GetProfile()
        {
            if (_settings.SaveFileMode != UIProfileSaveMode.None)
                UIMenuDataProfileSerializer.DeserializeData(out Generator.Profile, _settings.SaveFileName, _settings.SaveFileMode == UIProfileSaveMode.Outside);
        }

        [ContextMenu("Save Profile")]
        public void SaveProfile()
        {
            if (_settings.SaveFileMode != UIProfileSaveMode.None)
                UIMenuDataProfileSerializer.SerializeData(Generator.Profile, _settings.SaveFileName, _settings.SaveFileMode == UIProfileSaveMode.Outside);
        }

        private void DestroyAllChildren()
        {
            while (transform.childCount > 0)
                if (Application.isEditor)
                    DestroyImmediate(transform.GetChild(0).gameObject);
                else Destroy(transform.GetChild(0).gameObject);
        }

        private void InstantiateMenu()
        {
            var type = _type switch
            {
                UIMenuType.Hierarchical => Generator.Data.HierarchicalMenuTemplate,
                UIMenuType.Tabbed => Generator.Data.TabbedMenuTemplate,
                _ => null
            };

            var go = Instantiate(type, transform);
            go.name = _type.ToString() + " Menu UI Document";
        }
    }
}
