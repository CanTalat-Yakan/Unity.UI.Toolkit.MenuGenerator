using System;
using UnityEditor;
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
        private string _info;

        public UIProfileSaveMode SaveFileMode = UIProfileSaveMode.Inside;
        public string SaveFileName = "Menu";
        public bool SaveOnChange = true;

        [OnValueChanged("SaveFileMode")]
        public void OnSaveFileModeValueChanged()
        {
            switch (SaveFileMode)
            {
                case UIProfileSaveMode.None:
                    _info =
                        "No save file will be created. " +
                        "The profile will not persist between sessions and will not be saved to disk.";
                    break;
                case UIProfileSaveMode.Outside:
                    _info =
                        "A save file will be created outside the Assets/Build_Data folder, " +
                        "within the automatically created directory called Resources.";
                    break;
                case UIProfileSaveMode.Inside:
                    _info =
                        "A save file will be created inside the Assets/Build_Data folder, " +
                        "within the automatically created directory called Resources.";
                    break;
                default:
                    break;
            }
        }
    }

    public class UIMenu : MonoBehaviour
    {
        [SerializeField] private UIMenuSettings _settings = new();

        [Space]
        [SerializeField] private UIMenuType _type;

        public UIMenuData Data;

        public UIMenuDataProfile Profile => Generator.Profile;
        public UIMenuDataProfile DefaultProfile => Generator.Default;

        private UIMenuGenerator _generator;
        [HideInInspector] public UIMenuGenerator Generator => _generator ??= GetComponent<UIMenuGenerator>();

        public static Action<UIMenu> ShowEditor { get; set; }
        public Action<UIMenuData> SetData { get; private set; }

        public void Start()
        {
            if (Data == null)
                return;

            GetProfile();
            Generator.Profile.OnValueChanged += () => SaveProfile();
            Generator.PopulateRoot = () => Generator.Populate(true, Data.Name, Data.Root);
            Generator.Show();
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
                Selection.activeGameObject = gameObject;
            };

            ShowEditor?.Invoke(this);
#endif
        }

        [OnValueChanged("_type")]
        public void OnTypeValueChanged()
        {
            DestroyAllChildren();
            InstantiateMenu();

            GetProfile();
            SaveProfile();

            Generator.Initialize();
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
            Generator.Default ??= CreateProfileInstance("Default");
            foreach (var data in Data.Root)
                AddToDefaultProfileRecursively(data);

            if (_settings.SaveFileMode != UIProfileSaveMode.None)
            {
                var fileName = saveFileName ?? _settings.SaveFileName;
                var parentDirectory = _settings.SaveFileMode == UIProfileSaveMode.Outside;

                UIMenuDataProfileSerializer.DeserializeData(out Generator.Profile, fileName, parentDirectory);
                Generator.Profile.AddFrom(Generator.Default);
            }

            return Generator.Profile ??= Generator.Default;
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

        private UIMenuDataProfile CreateProfileInstance(string name = "Profile")
        {
            var profile = ScriptableObject.CreateInstance<UIMenuDataProfile>();
            profile.name = name + " AutoCreated";
            return profile;
        }

        private void AddToDefaultProfileRecursively(ScriptableObject data)
        {
            var field = data.GetType().GetField("Data");
            var children = field?.GetValue(data) as ScriptableObject[];
            if (children != null && children.Length > 0)
                foreach (var child in children)
                    AddToDefaultProfileRecursively(child);

            var type = data as UIGeneratorTypeTemplate;
            type?.ProfileAddDefault(Generator.Default);
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

            if (type == null)
            {
                Debug.LogError("No template found for the selected menu type: " + _type);
                return;
            }

            var go = Instantiate(type, transform);
            go.name = _type.ToString() + " Menu UI Document";
        }
    }
}
