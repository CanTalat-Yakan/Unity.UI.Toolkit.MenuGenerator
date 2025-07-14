using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionCategoryDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuSelectionCategoryData>
    {
        [Space]
        public int Default;

        public override void ApplyDynamicConfiguration() =>
            Data.Default = Default;
    }
}
