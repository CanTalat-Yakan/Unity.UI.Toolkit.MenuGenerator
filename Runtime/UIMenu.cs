using System;
using System.Collections;
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
        public static Dictionary<string, UIMenu> RegisteredMenus { get; private set; } = new();

        [Space]
        public UIMenuType Type;
        [OnValueChanged(nameof(Type))] public void OnTypeValueChanged() => Initialize();

        [Space]
        public string Name = "Menu";
        public UIMenuData Data;

        [OnValueChanged(nameof(Name))]
        public void OnNameValueChanged()
        {
            var provider = GetComponent<UIMenuProfileProvider>();
            if (provider != null)
                provider.Name = Name;
        }

        [HideInInspector] public UIMenuGenerator Generator => _generator ??= this.GetOrAddComponent<UIMenuGenerator>();
        [NonSerialized] private UIMenuGenerator _generator;

        [HideInInspector] public UIMenuProfileProvider Provider => _provider ??= this.GetOrAddComponent<UIMenuProfileProvider>();
        [NonSerialized] private UIMenuProfileProvider _provider;

        public UIMenuProfile Profile => Provider.Profile;
        public UIMenuProfile DefaultProfile => Provider.Default;

        public void Awake()
        {
            if (Data == null)
                return;

            RegisteredMenus.Add(Name, this);
            Generator.PopulateRoot = () => Generator.Populate(true, Data.Name, Data.Root);
        }

        public IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            Generator.Show();
            yield return null;
        }

#if UNITY_EDITOR
        private void OnDisable()
        {
            if (Application.isPlaying)
                OnExitPlayMode();
        }

        public static Action<UIMenu> ShowEditor { get; set; }
        public Action<UIMenuData> SetData { get; private set; }

        [Button()]
        public void OpenMenuBuilder()
        {
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
        }
#endif

        public static bool TryGetProfile(string name, out UIMenuProfile profile) =>
            UIMenuProfileProvider.TryGetProfile(name, out profile);

        public static bool TryGetData<T>(string menuName, string dataReference, out T outputData) where T : UIMenuTypeDataBase
        {
            outputData = default;
            if (RegisteredMenus.TryGetValue(menuName, out var menu) && menu.Data is not null)
                return menu.Data.TryGetValue(dataReference, out outputData);
            return false;
        }

        public void Initialize()
        {
            this.DestroyAllChildren();

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
    }
}