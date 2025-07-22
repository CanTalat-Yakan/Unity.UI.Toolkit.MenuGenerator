using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuCategoryDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuCategoryData>
    {
        [Space]
        public UIMenuData MenuData;

        public override void ApplyDynamicConfiguration() =>
            Data.Data = MenuData.Root;
    }
}
