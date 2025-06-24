using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuData : ScriptableObject
    {
        [HideInInspector] public string Name = "Menu";
        [HideInInspector] public ScriptableObject[] Root;
    }
}
