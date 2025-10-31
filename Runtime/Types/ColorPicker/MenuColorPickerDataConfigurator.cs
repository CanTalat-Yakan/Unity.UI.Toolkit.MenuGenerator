using UnityEngine;

namespace UnityEssentials
{
    public class MenuColorPickerDataConfigurator : MenuTypeDataConfiguratorBase<MenuColorPickerData>
    {
        [Space]
        public Color Default;

        public override void ApplyDynamicConfiguration() =>
            Data.Default = Default;
    }
}