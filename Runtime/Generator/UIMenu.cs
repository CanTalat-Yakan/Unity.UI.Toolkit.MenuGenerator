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
            "and is stored outside the Asset folder, within the Resources directory.\n" +
            "Make sure to include the Resources directory and its contents in your Build folder!";

        public UIProfileSaveMode SaveFileMode = UIProfileSaveMode.Outside;
        public string SaveFileName = "UIMenuSettings";
    }

    public class UIMenu : MonoBehaviour
    {
        public static Action<UIMenu> ShowUIBuilder { get; set; }
        public Action<UIMenuData> SetUIMenuData { get; private set; }

        [SerializeField] private UIMenuSettings _settings = new();

        [Space]
        [SerializeField] private UIMenuType _type;

        public UIMenuData Data;

        [HideInInspector] public UIMenuGenerator Generator => GetComponent<UIMenuGenerator>();

        public void Start()
        {
            if (Data == null)
                return;

            Generator.PopulateHierarchy(true, Data.Name, Data.Root);
        }

        [Button()]
        public void OpenUIBuilder()
        {
#if UNITY_EDITOR
            if (Data == null || Data.Equals(null))
                Data = CreateDefault();

            SetUIMenuData = (data) =>
            {
                Data = data;
                UnityEditor.Selection.activeGameObject = gameObject;
            };

            ShowUIBuilder?.Invoke(this);
#endif
        }

        private UIMenuData CreateDefault()
        {
            var data = ScriptableObject.CreateInstance<UIMenuData>();
            data.name = "Menu Data AutoCreated";
            data.Name = "Menu";
            data.Root = Array.Empty<ScriptableObject>();
            return data;
        }

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

        public UIMenuDataProfile GetProfile(string saveFileName = null)
        {
            if (_settings.SaveFileMode != UIProfileSaveMode.None)
                UIMenuDataProfileSerializer.DeserializeData(out Generator.Profile,
                    saveFileName ??= _settings.SaveFileName, _settings.SaveFileMode == UIProfileSaveMode.Outside);

            return Generator.Profile ??= ScriptableObject.CreateInstance<UIMenuDataProfile>();
        }

        public void SaveProfile(string saveFileName = null)
        {
            if (_settings.SaveFileMode != UIProfileSaveMode.None)
                UIMenuDataProfileSerializer.SerializeData(Generator.Profile,
                    saveFileName ??= _settings.SaveFileName, _settings.SaveFileMode == UIProfileSaveMode.Outside);
        }

        [HideInInspector] public bool ShowAdvancedSettings = false;
        [ContextMenu("Show Advanced Settings")]
        private void ShowAdvanced()
        {
            ShowAdvancedSettings = !ShowAdvancedSettings;
            GetComponent<UIMenuGenerator>().hideFlags = ShowAdvancedSettings ? HideFlags.None : HideFlags.HideInInspector;
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
