using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionGroupDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuSelectionGroupData>
    {
        [Space]
        public bool Reverse;
        public UIMenuSelectionData Selections;

        public override void ApplyDataValues()
        {
            Data.Reverse = Reverse;
            Data.Selections = Selections;
        }
    }
}
