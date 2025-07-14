using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputDataConfigurator : MonoBehaviour
    {
        public string MenuName;
        public string Reference;

        [Space]
        public string Default;

        private UIMenuInputData _inputData;
        [HideInInspector] public UIMenuInputData InputData => _inputData;

        public void Awake()
        {
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetData(Reference, out _inputData) ?? false)
            {
                InputData.IsDynamic = true;
                InputData.Default = Default;
            }
        }
    }
}
