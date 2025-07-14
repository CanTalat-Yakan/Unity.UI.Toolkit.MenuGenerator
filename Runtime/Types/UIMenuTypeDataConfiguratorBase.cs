using UnityEngine;

namespace UnityEssentials
{
    public abstract class UIMenuTypeDataConfiguratorBase<T> : MonoBehaviour where T : UIMenuTypeDataBase
    {
        public string MenuName;
        public string Reference;

        private T _data;
        [HideInInspector] public T Data => _data;

        public void Awake()
        {
            if (UIMenu.TryGetValue(MenuName, Reference, out _data))
            {
                Data.IsDynamic = true;
                ApplyDynamicConfiguration();
            }
        }

        public abstract void ApplyDynamicConfiguration();
    }
}
