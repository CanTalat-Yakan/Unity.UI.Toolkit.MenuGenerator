using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuData : ScriptableObject
    {
        [HideInInspector, SerializeField] public string Name = "Menu";
        [HideInInspector, SerializeField] public ScriptableObject[] Root = { };
    }
}
