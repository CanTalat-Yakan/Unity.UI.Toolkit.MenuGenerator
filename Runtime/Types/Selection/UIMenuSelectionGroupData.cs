using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionGroupData : UIMenuTypeDataBase
    {
        [Space]
        public UIMenuSelectionData Selections;

        public UIMenuSelectionData GetSelections() =>
            Selections;

        public override object GetDefault() => null;

        public override void ApplyDynamicReset() =>
            Selections = null;
    }
}
