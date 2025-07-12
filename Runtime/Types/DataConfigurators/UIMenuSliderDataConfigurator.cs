using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSliderDataConfigurator : MonoBehaviour
    {
        public string MenuName;
        public string Reference;

        [Space]
        public bool IsFloat;
        public float MinRange = 0;
        public float MaxRange = 100;

        [Space]
        public float Default;

        private UIMenuSliderData _sliderData;
        [HideInInspector] public UIMenuSliderData SliderData => _sliderData;

        public void Awake()
        {
                if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                    return;

            if (menu.Data?.GetDataByReference(Reference, out _sliderData) ?? false)
            {
                SliderData.IsDynamic = true;
                SliderData.IsFloat = IsFloat;
                SliderData.MinRange = MinRange;
                SliderData.MaxRange = MaxRange;
                SliderData.Default = Default;
            }
        }
    }
}
