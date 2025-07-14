using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorPickerDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuColorPickerData>
    {
        [Space]
        public Color Default;

        public override void ApplyDynamicConfiguration() =>
            Data.Default = Default;
    }
}
