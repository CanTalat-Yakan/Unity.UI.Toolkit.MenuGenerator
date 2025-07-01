using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIGeneratorTypeTemplate : ScriptableObject
    {
        [HideInInspector] public GUID? ID;

        public void Initialize() =>
            ID ??= GUID.Generate();
    }
}
