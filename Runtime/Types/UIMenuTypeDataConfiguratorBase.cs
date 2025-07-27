using UnityEngine;

namespace UnityEssentials
{
    [DefaultExecutionOrder(-1000)]
    public abstract class UIMenuTypeDataConfiguratorBase<T> : MonoBehaviour where T : UIMenuTypeDataBase
    {
        public string MenuName;
        public string DataReference;

        private T _data;
        [HideInInspector] public T Data => _data;

        public void Awake()
        {
            if (string.IsNullOrEmpty(MenuName) || string.IsNullOrEmpty(DataReference))
                return;

            ConfigureMenuData();
        }

        public void ConfigureMenuData()
        {
            if (UIMenu.TryGetData(MenuName, DataReference, out _data))
            {
                Data.IsDynamic = true;
                ApplyDynamicConfiguration();
            }
        }

        public abstract void ApplyDynamicConfiguration();
    }
}