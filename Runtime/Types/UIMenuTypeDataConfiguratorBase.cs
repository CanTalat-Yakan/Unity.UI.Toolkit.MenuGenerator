using UnityEngine;

namespace UnityEssentials
{
    [DefaultExecutionOrder(-1000)]
    public abstract class UIMenuTypeDataConfiguratorBase<T> : MonoBehaviour where T : UIMenuTypeDataBase
    {
        public string MenuName;
        public string DataReference;

        [HideInInspector] public T Data => _data;
        private T _data;

        [HideInInspector] public UIMenu Menu => _menu;
        private UIMenu _menu;

        public void Awake()
        {
            if (string.IsNullOrEmpty(MenuName) || string.IsNullOrEmpty(DataReference))
                return;

            ConfigureMenuData();
        }

        public void ConfigureMenuData()
        {
            UIMenu.RegisteredMenus.TryGetValue(MenuName, out _menu);

            if (UIMenu.TryGetData(MenuName, DataReference, out _data))
            {
                Data.IsDynamic = true;
                ApplyDynamicConfiguration();
            }
        }

        public abstract void ApplyDynamicConfiguration();
    }
}