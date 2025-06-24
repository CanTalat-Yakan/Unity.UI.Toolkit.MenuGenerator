using UnityEngine;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "SelectionDataCollection_", menuName = "UI/Selection Collection", order = 0)]
    public class UIMenuSelectionDataCollection : ScriptableObject
    {
        public string Name;

        public UIMenuSelectionData[] Data;

        public UIMenuSelectionDataCollection SetName(string name)
        {
            base.name = name;
            Name = name;
            return this;
        }
    }
}