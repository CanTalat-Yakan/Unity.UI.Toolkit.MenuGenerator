using System.Linq;
using UnityEngine;

namespace UnityEssentials
{
    public class MenuSelectionGroupData : MenuTypeDataBase
    {
        [Space]
        public MenuSelectionData Selections;

        public MenuSelectionData GetSelections() =>
            Selections;

        public override object GetDefault() => null;

        public override void ApplyDynamicReset() =>
            Selections = null;
    }
}