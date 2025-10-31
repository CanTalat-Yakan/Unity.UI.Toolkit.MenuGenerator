using UnityEngine;

namespace UnityEssentials
{
    public class MenuToggleDataConfigurator : MenuTypeDataConfiguratorBase<MenuToggleData>
    {
        [Space]
        public bool Default;

        public override void ApplyDynamicConfiguration() =>
            Data.Default = Default;
    }
}