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
        [OnValueChanged("SaveFileMode")]
        public void OnSaveFileModeValueChanged()
        {
            switch (SaveFileMode)
            {
                case UIProfileSaveMode.None:
                    _info = "No save file will be created. " +
                        "The profile will not persist between sessions and will not be saved to disk.";
                    break;
                case UIProfileSaveMode.Outside:
                    _info = "A save file will be created outside the Asset folder, " +
                        "within the Resources directory. Make sure to include the Resources directory and its contents in your Build folder!";
                    break;
                case UIProfileSaveMode.Inside:
                    _info = "A save file will be created inside the Asset folder, " +
                        "within the Resources directory. Make sure to include the Resources directory and its contents in your Build folder!";
                    break;
                default:
                    break;
            }
        }

        public string SaveFileName = "Menu";
        public bool SaveOnChange = true;
        [OnValueChanged("SaveOnChange")]
        public void OnChanged()
        {
            Debug.Log("TEst");
        }
    }

    public class UIMenu : MonoBehaviour
    {
        public static Action<UIMenu> ShowEditor { get; set; }
        public Action<UIMenuData> SetData { get; private set; }

        [SerializeField] private UIMenuSettings _settings = new();

        [Space]
        [SerializeField] private UIMenuType _type;
        [OnValueChanged("_type")]
        public void OnTypeValueChanged()
        {
            DestroyAllChildren();
            InstantiateMenu();

            GetProfile();
            SaveProfile();

            Generator.Initialize();
        }

        public UIMenuData Data;

        [HideInInspector] public UIMenuGenerator Generator => GetComponent<UIMenuGenerator>();

        public void Start()
        {
            if (Data == null)
                return;

            GetProfile();
            Generator.Profile.OnValueChanged += () => SaveProfile();
            Generator.Populate(true, Data.Name, Data.Root);
        }

        [Button()]
        public void OpenMenuBuilder()
        {
#if UNITY_EDITOR
            if (Data == null || Data.Equals(null))
                Data = CreateDefault();

            SetData = (data) =>
            {
                if (this == null) 
                    return;

                Data = data;
                UnityEditor.Selection.activeGameObject = gameObject;
            };

            ShowEditor?.Invoke(this);
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

        public UIMenuDataProfile GetProfile(string saveFileName = null)
        {
            if (_settings.SaveFileMode != UIProfileSaveMode.None)
                UIMenuDataProfileSerializer.DeserializeData(out Generator.Profile,
                    saveFileName ??= _settings.SaveFileName, 
                    _settings.SaveFileMode == UIProfileSaveMode.Outside);

            return Generator.Profile ??= ScriptableObject.CreateInstance<UIMenuDataProfile>();
        }

        public void SaveProfile(string saveFileName = null)
        {
            if (_settings.SaveFileMode != UIProfileSaveMode.None)
                UIMenuDataProfileSerializer.SerializeData(Generator.Profile,
                    saveFileName ??= _settings.SaveFileName, 
                    _settings.SaveFileMode == UIProfileSaveMode.Outside);
        }

        [HideInInspector] public bool ShowAdvancedSettings = false;
        [ContextMenu("Show Generator")]
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

            if(type == null)
            {
                Debug.LogError("No template found for the selected menu type: " + _type);
                return;
            }

            var go = Instantiate(type, transform);
            go.name = _type.ToString() + " Menu UI Document";
        }
    }
}
