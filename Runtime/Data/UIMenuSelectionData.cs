using System;
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

    [CreateAssetMenu(fileName = "SelectionData_", menuName = "UI/Selection", order = 0)]
    public class UIMenuSelectionData : ScriptableObject
    {
        public int ID;

        public UIMenuSelectionDataElement[] Data;
    }
}
