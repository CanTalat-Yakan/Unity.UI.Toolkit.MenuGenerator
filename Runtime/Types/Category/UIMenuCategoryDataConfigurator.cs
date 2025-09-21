using UnityEngine;

namespace UnityEssentials
{
    [DefaultExecutionOrder(-1009)]
    public class UIMenuCategoryDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuCategoryData>
    {
        [Space]
        public UIMenuData MenuData;

        public override void ApplyDynamicConfiguration()
        {
            if (MenuData != null)
                Data.Data = MenuData.Root;
        }
    }
}