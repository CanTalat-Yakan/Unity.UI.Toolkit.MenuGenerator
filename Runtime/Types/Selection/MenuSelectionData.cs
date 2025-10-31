using System;
using UnityEngine;

namespace UnityEssentials
{
    [Serializable]
    public class MenuSelectionDataElement
    {
        public string Name;

        [Space]
        public Texture2D Texture;
        public GameObject Source;
    }

    [CreateAssetMenu(fileName = "SelectionData_", menuName = "Menu/Selection", order = 0)]
    public class MenuSelectionData : ScriptableObject
    {
        [Tooltip("The starting index is added to the element's index to determine the final Selection ID")]
        public int StartIndexID;

        public MenuSelectionDataElement[] Data;
    }
}