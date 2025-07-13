using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class UIMenuSelectionDataElement
    {
        public string Name;

        [Space]
        public Texture2D Texture;
        public GameObject Source;
    }

    [CreateAssetMenu(fileName = "SelectionData_", menuName = "Menu/Selection", order = 0)]
    public class UIMenuSelectionData : ScriptableObject
    {
        [Tooltip("The starting index is added to the element's index to determine the final Selection ID")]
        public int StartIndexID;

        public UIMenuSelectionDataElement[] Data;
    }

    public class UIMenuSelectionGroupData : UIMenuTypeBase
    {
        [Space]
        public bool Reverse;
        public UIMenuSelectionData[] Selections;

        public List<UIMenuSelectionData> GetSelections()
        {
            List<UIMenuSelectionData> list = Selections != null ? new(Selections) : new();
            if (Reverse) list.Reverse();
            return list;
        }

        public override void ApplyDynamicReset() =>
            Selections = Array.Empty<UIMenuSelectionData>();
    }
}
