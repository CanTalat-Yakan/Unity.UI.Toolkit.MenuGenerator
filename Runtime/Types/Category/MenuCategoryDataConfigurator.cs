using UnityEngine;

namespace UnityEssentials
{
    [DefaultExecutionOrder(-1009)]
    public class MenuCategoryDataConfigurator : MenuTypeDataConfiguratorBase<MenuCategoryData>
    {
        [Space]
        public MenuData MenuData;

        public override void ApplyDynamicConfiguration()
        {
            if (MenuData != null)
                Data.Data = MenuData.Root;
        }
    }
}