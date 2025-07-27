using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuInputData>
    {
        [Space]
        public string Default;

        public override void ApplyDynamicConfiguration() =>
            Data.Default = Default;
    }
}