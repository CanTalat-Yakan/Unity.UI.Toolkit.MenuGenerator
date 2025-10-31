using UnityEngine;

namespace UnityEssentials
{
    public class MenuSelectionCategoryDataConfigurator : MenuTypeDataConfiguratorBase<MenuSelectionCategoryData>
    {
        [Space]
        public int Default;

        public override void ApplyDynamicConfiguration() =>
            Data.Default = Default;
    }
}