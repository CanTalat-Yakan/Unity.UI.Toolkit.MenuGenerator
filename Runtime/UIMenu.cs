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
        public UIMenuSettings Settings = new();

        [Space]
        public UIMenuType Type;
        [OnValueChanged("Type")] public void OnTypeValueChanged() => Initialize();

        public string Name = "Menu";
        public UIMenuData Data;

        public UIMenuDataProfile Profile => Provider.Profile;
        public UIMenuDataProfile DefaultProfile => Provider.Default;

        [HideInInspector] public UIMenuGenerator Generator => _generator ??= GetComponent<UIMenuGenerator>();
        private UIMenuGenerator _generator;

        [HideInInspector] public UIMenuDataProfileProvider Provider => _provider ??= GetComponent<UIMenuDataProfileProvider>();
        private UIMenuDataProfileProvider _provider;

        public static Action<UIMenu> ShowEditor { get; set; }
        public Action<UIMenuData> SetData { get; private set; }

        public void OnEnable()
        {
            if (Data == null)
                return;

            Generator.PopulateRoot = () => Generator.Populate(true, Data.Name, Data.Root);
        }

        public void Start() =>
            Generator.Show();

#if UNITY_EDITOR
        private void OnDisable()
        {
            if (Application.isPlaying)
                OnExitPlayMode();
        }

        private void OnExitPlayMode()
        {
            foreach (var item in Data.GetDataItems())
                if (item is UIGeneratorTypeTemplate dataTemplate)
                    if(dataTemplate.IsDynamic)
                    {
                        dataTemplate.ApplyDynamicReset();
                        dataTemplate.IsDynamic = false;
                    }
        }
#endif

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

        public void Initialize()
        {
            DestroyAllChildren();
            InstantiateMenu();

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

        private void DestroyAllChildren()
        {
            while (transform.childCount > 0)
                if (Application.isEditor)
                    DestroyImmediate(transform.GetChild(0).gameObject);
                else Destroy(transform.GetChild(0).gameObject);
        }

        private void InstantiateMenu()
        {
            var type = Type switch
            {
                UIMenuType.Hierarchical => Generator.Data.HierarchicalMenuTemplate,
                UIMenuType.Tabbed => Generator.Data.TabbedMenuTemplate,
                _ => null
            };

            if (type == null)
            {
                Debug.LogError("No template found for the selected menu type: " + Type);
                return;
            }

            var go = Instantiate(type, transform);
            go.name = Type.ToString() + " Menu UI Document";
        }
    }
}
