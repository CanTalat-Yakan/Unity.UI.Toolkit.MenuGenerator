using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionGroupData : UIMenuTypeDataBase
    {
        [Space]
        public bool Reverse;
        public UIMenuSelectionData Selections;

        public UIMenuSelectionData GetSelections()
        {
            if (!Reverse)
                return Selections;

            var list = Selections.Data.ToList();
            list.Reverse();
            return new() { StartIndexID = Selections.StartIndexID, Data = list.ToArray() };
        }

        public override object GetDefault() => null;

        public override void ApplyDynamicReset()
        {
            Reverse = false;
            Selections = null;
        }
    }
}
