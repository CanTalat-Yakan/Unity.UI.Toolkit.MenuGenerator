using UnityEngine;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "UIMenuData_", menuName = "UI/Menu", order = -1)]
    public class UIMenuData : ScriptableObject
    {
        public string Name = "Root";
        public ScriptableObject[] Root;
    }
}
