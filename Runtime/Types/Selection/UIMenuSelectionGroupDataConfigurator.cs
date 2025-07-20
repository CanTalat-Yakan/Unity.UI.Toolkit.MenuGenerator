using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionGroupDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuSelectionGroupData>
    {
        [Space]
        public UIMenuSelectionData Selections;

        public override void ApplyDynamicConfiguration()
        {
            Data.Selections = Selections;
        }
    }
}
