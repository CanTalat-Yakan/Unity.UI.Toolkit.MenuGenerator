using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSpacerDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuSpacerData>
    {
        [Space]
        public int Height = 20;

        public override void ApplyDynamicConfiguration() =>
            Data.Height = Height;
    }
}