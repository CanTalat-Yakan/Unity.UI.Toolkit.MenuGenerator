using UnityEngine;

namespace UnityEssentials
{
    public class MenuSpacerDataConfigurator : MenuTypeDataConfiguratorBase<MenuSpacerData>
    {
        [Space]
        public int Height = 20;

        public override void ApplyDynamicConfiguration() =>
            Data.Height = Height;
    }
}