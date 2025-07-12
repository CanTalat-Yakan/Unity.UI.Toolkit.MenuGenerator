using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorSliderDataConfigurator : MonoBehaviour
    {
        public string MenuName;
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
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetDataByReference(Reference, out _colorSliderData) ?? false)
            {
                ColorSliderData.IsDynamic = true;
                ColorSliderData.Gradient = Gradient;
                ColorSliderData.Default = Default;
            }
        }
    }
}
