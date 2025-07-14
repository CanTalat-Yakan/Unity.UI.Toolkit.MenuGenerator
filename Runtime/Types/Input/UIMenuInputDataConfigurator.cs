using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuInputData>
    {
        [Space]
        public string Default;

        public override void ApplyDataValues() =>
            Data.Default = Default;
    }
}
