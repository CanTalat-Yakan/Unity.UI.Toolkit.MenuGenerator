using UnityEngine;

namespace UnityEssentials
{
    public class MenuSelectionGroupDataConfigurator : MenuTypeDataConfiguratorBase<MenuSelectionGroupData>
    {
        [Space]
        public MenuSelectionData Selections;

        public override void ApplyDynamicConfiguration()
        {
            Data.Selections = Selections;
        }
    }
}