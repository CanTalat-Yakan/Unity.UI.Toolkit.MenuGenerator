using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionGroupDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuSelectionGroupData>
    {
        [Space]
        public bool Reverse;
        public UIMenuSelectionData Selections;

        public override void ApplyDynamicConfiguration()
        {
            Data.Reverse = Reverse;
            Data.Selections = Selections;
        }
    }
}
