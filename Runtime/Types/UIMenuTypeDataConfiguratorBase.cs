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
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetData(Reference, out _data) ?? false)
            {
                Data.IsDynamic = true;
                ApplyDataValues();
            }
        }

        public abstract void ApplyDataValues();
    }
}
