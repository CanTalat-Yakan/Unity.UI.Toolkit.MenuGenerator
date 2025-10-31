using UnityEngine;

namespace UnityEssentials
{
    public class MenuInputDataConfigurator : MenuTypeDataConfiguratorBase<MenuInputData>
    {
        [Space]
        public string Default;

        public override void ApplyDynamicConfiguration() =>
            Data.Default = Default;
    }
}