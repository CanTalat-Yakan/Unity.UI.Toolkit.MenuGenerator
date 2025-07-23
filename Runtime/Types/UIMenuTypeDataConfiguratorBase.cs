using UnityEngine;

namespace UnityEssentials
{
    [DefaultExecutionOrder(1001)]
    public abstract class UIMenuTypeDataConfiguratorBase<T> : MonoBehaviour where T : UIMenuTypeDataBase
    {
        public string MenuName;
        public string DataReference;

        private T _data;
        [HideInInspector] public T Data => _data;

        public void Awake()
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
