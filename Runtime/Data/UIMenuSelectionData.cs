using UnityEngine;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "SelectionData_", menuName = "UI/Selection", order = 0)]
    public class UIMenuSelectionData : ScriptableObject
    {
        public string Name;
        public int ID;

        [Space]
        public Texture2D Texture;
        public GameObject Source;
    }
}
