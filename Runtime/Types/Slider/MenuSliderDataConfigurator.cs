using UnityEngine;

namespace UnityEssentials
{
    public class MenuSliderDataConfigurator : MenuTypeDataConfiguratorBase<MenuSliderData>
    {
        [Space]
        public bool IsFloat;
        public float MinValue = 0;
        public float MaxValue = 100;

        [Space]
        public float Default;

        public override void ApplyDynamicConfiguration()
        {
            Data.IsFloat = IsFloat;
            Data.MinValue = MinValue;
            Data.MaxValue = MaxValue;
            Data.Default = Default;
        }
    }
}