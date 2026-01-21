using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public enum MenuProfileSaveMode
    {
        None,
        Outside,
        Inside,
    }

    public enum MenuType
    {
        Hierarchical,
        Tabbed,
    }

    [DefaultExecutionOrder(-1010)]
    public class Menu : MonoBehaviour
    {
        public static Dictionary<string, Menu> RegisteredMenus { get; private set; } = new();

        [Space]
        public MenuType Type;
        [OnValueChanged(nameof(Type))] public void OnTypeValueChanged() => Initialize();

        [Space]
        public string Name = "Menu";
        public MenuData Data;

        public MenuGenerator Generator => _generator ??= this.GetOrAddComponent<MenuGenerator>();
        [NonSerialized] private MenuGenerator _generator;

        public SettingsProfileManager ProfileManager = SettingsProfileManager.GetOrCreate("MenuProfileManager");
        
        public void Awake()
        {
            ValidateMenuData();
            RegisteredMenus.Add(Name, this);
            Generator.PopulateRoot = () => Generator.Populate(true, Data.Name, Data.Root);
        }

        public IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            ValidateMenuData();
            Generator.Show();
            yield return null;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        public static void ClearRegisteredMenus() =>
            RegisteredMenus.Clear();

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                ClearRegisteredMenus();
                OnExitPlayMode();
            }
        }

        public static Action<Menu> ShowEditor { get; set; }
        public Action<MenuData> SetData { get; private set; }

        [Button]
        public void OpenMenuBuilder()
        {
            ValidateMenuData();
            SetData = data =>
            {
                if (this == null)
                    return;

                Data = data;
                Selection.activeGameObject = gameObject;
            };
            ShowEditor?.Invoke(this);
        }
#endif

        public static bool TryGetData<T>(string menuName, string dataReference, out T outputData) where T : MenuTypeDataBase
        {
            outputData = default;
            if (RegisteredMenus.TryGetValue(menuName, out var menu) && menu.Data is not null)
                return menu.Data.TryGetValue(dataReference, out outputData);
            return false;
        }

        public void Initialize()
        {
            this.DestroyAllChildren<UIDocument>();

            InstantiateMenu();
        }

        private void ValidateMenuData()
        {
            if (Data == null || Data.Equals(null))
                Data = CreateDefault();
        }

        private MenuData CreateDefault()
        {
            var data = ScriptableObject.CreateInstance<MenuData>();
            data.name = "Menu Data AutoCreated";
            data.Name = Name;
            data.Root = Array.Empty<ScriptableObject>();
            return data;
        }

        private void InstantiateMenu()
        {
            var instanceName = Type + " Menu UI Document";
            var prefab = Type switch
            {
                MenuType.Hierarchical => "UnityEssentials_Prefab_HierarchicalMenu",
                MenuType.Tabbed => "UnityEssentials_Prefab_TabbedMenu",
                _ => null
            };

            if (prefab == null)
                return;

            var menu = AssetResolver.InstantiatePrefab(prefab, instanceName, transform);
            menu.transform.SetSiblingIndex(0);
        }

        private void OnExitPlayMode()
        {
            if (Data == null)
                return;

            foreach (var data in Data.EnumerateAllData())
                if (data is MenuTypeDataBase dataTemplate)
                    if (dataTemplate.IsDynamic)
                    {
                        dataTemplate.ApplyDynamicReset();
                        dataTemplate.IsDynamic = false;
                    }
        }
    }
}