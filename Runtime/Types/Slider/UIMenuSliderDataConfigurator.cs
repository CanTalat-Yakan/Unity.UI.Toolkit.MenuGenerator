using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSliderDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuSliderData>
    {
        [Space]
        public bool IsFloat;
        public float MinRange = 0;
        public float MaxRange = 100;

        [Space]
        public float Default;

        public override void ApplyDataValues()
        {
            Data.IsFloat = IsFloat;
            Data.MinRange = MinRange;
            Data.MaxRange = MaxRange;
            Data.Default = Default;
        }
    }
}
