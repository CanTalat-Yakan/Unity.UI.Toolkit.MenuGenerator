using UnityEngine;

namespace UnityEssentials
{
    [DefaultExecutionOrder(-1000)]
    public abstract class MenuTypeDataConfiguratorBase<T> : MonoBehaviour where T : MenuTypeDataBase
    {
        public string MenuName;
        public string DataReference;

        [HideInInspector] public T Data => _data;
        private T _data;

        [HideInInspector] public Menu Menu => _menu;
        private Menu _menu;

        public void Awake()
        {
            if (string.IsNullOrEmpty(MenuName) || string.IsNullOrEmpty(DataReference))
                return;

            ConfigureMenuData();
        }

        public void ConfigureMenuData()
        {
            Menu.RegisteredMenus.TryGetValue(MenuName, out _menu);

            if (Menu.TryGetData(MenuName, DataReference, out _data))
            {
                Data.IsDynamic = true;
                ApplyDynamicConfiguration();
            }
        }

        public abstract void ApplyDynamicConfiguration();
    }
}