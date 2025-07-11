using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputDataConfigurator : MonoBehaviour
    {
        public UIMenuData MenuData;
        public string Reference;

        [Space]
        public string Default;

        private UIMenuInputData _inputData;
        [HideInInspector] public UIMenuInputData InputData => _inputData;

        public void Awake()
        {
            if (MenuData?.GetDataByReference(Reference, out _inputData) ?? false)
            {
                InputData.IsDynamic = true;
                InputData.Default = Default;
            }
        }
    }
}
