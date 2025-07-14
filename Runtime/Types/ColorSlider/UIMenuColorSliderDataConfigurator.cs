using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorSliderDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuColorSliderData>
    {
        [Space]
        public Gradient Gradient;

        [Space]
        [Range(0, 100)]
        public float Default;

        public override void ApplyDataValues()
        {
            Data.Gradient = Gradient;
            Data.Default = Default;
        }
    }
}
