using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorSliderDataConfigurator : MonoBehaviour
    {
        public UIMenuData MenuData;
        public string Reference;

        [Space]
        public Gradient Gradient;

        [Space]
        [Range(0, 100)]
        public float Default;

        private UIMenuColorSliderData _colorSliderData;
        [HideInInspector] public UIMenuColorSliderData ColorSliderData => _colorSliderData;

        public void Awake()
        {
            if (MenuData?.GetDataByReference(Reference, out _colorSliderData) ?? false)
            {
                ColorSliderData.IsDynamic = true;
                ColorSliderData.Gradient = Gradient;
                ColorSliderData.Default = Default;
            }
        }
    }
}
