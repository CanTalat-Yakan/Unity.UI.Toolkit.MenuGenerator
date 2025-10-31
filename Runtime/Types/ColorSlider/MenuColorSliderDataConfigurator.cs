using UnityEngine;

namespace UnityEssentials
{
    public class MenuColorSliderDataConfigurator : MenuTypeDataConfiguratorBase<MenuColorSliderData>
    {
        [Space]
        public Gradient Gradient;

        [Space]
        [Range(0, 100)]
        public float Default;

        public override void ApplyDynamicConfiguration()
        {
            Data.Gradient = Gradient;
            Data.Default = Default;
        }
    }
}