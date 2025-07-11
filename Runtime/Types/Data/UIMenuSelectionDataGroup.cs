using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionDataGroup : UIMenuGeneratorTypeTemplate
    {
        [Space]
        public bool Reverse;
        public UIMenuSelectionData[] Selections;

        public List<UIMenuSelectionData> GetSelections()
        {
            var list = Selections != null ? new List<UIMenuSelectionData>(Selections) : new List<UIMenuSelectionData>();
            if (Reverse) list.Reverse();
            return list;
        }

        public override void ApplyDynamicReset() =>
            Selections = Array.Empty<UIMenuSelectionData>();
    }
}
