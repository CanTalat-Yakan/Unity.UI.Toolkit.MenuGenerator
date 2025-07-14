using System;
using System.Collections.Generic;
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

    public class UIMenu : MonoBehaviour
    {
        public static Dictionary<string, UIMenu> Instances { get; private set; } = new();

        public static bool TryGetValue<T>(string menuName, string dataReference, out T outputData) where T : UIMenuTypeDataBase
        {
            outputData = default;
            if (!Instances.TryGetValue(menuName, out var menu) || menu.Data is null)
                return false;
            return menu.Data.TryGetValue(dataReference, out outputData);
        }

        [Space]
        public UIMenuType Type;
        [OnValueChanged("Type")] public void OnTypeValueChanged() => Initialize();

        [Space]
        public string Name = "Menu";
        public UIMenuData Data;

        [OnValueChanged("Name")]
        public void OnNameValueChanged() => GetComponent<UIMenuProfileProvider>().Name = Name;

        public UIMenuProfile Profile => Provider.Profile;
        public UIMenuProfile DefaultProfile => Provider.Default;

        [HideInInspector] public UIMenuGenerator Generator => _generator ??= this.GetOrAddComponent<UIMenuGenerator>();
        private UIMenuGenerator _generator;

        [HideInInspector] public UIMenuProfileProvider Provider => _provider ??= this.GetOrAddComponent<UIMenuProfileProvider>();
        private UIMenuProfileProvider _provider;

        public static Action<UIMenu> ShowEditor { get; set; }
        public Action<UIMenuData> SetData { get; private set; }

        public void OnEnable()
        {
            if (Data == null)
                return;

            Instances.Add(Name, this);

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
            foreach (var data in Data.EnumerateAllData())
                if (data is UIMenuTypeDataBase dataTemplate)
                    if (dataTemplate.IsDynamic)
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
            var name = Type.ToString() + " Menu UI Document";
            var prefab = Type switch
            {
                UIMenuType.Hierarchical => "UnityEssentials_Prefab_HierarchicalMenu",
                UIMenuType.Tabbed => "UnityEssentials_Prefab_TabbedMenu",
                _ => null
            };

            if (prefab == null)
                return;

            ResourceLoader.InstantiatePrefab(prefab, name, transform);
        }
    }
}
