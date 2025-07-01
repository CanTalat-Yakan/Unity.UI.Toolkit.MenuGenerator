using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIGeneratorTypeTemplate : ScriptableObject
    {
        [HideInInspector] public bool HasReference;
        [HideInInspector] public GUID? ID;
        [HideInInspector] public string Name;
        public string Reference;

        public static T Initialize<T>(string name = null, string uniqueName = null, bool hasReference = true) where T : UIGeneratorTypeTemplate
        {
            var generatorType = CreateInstance<T>();
            generatorType.HasReference = hasReference;
            generatorType.ID ??= GUID.Generate();
            generatorType.SetName(name ?? string.Empty, uniqueName);
            return generatorType;
        }

        public void SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            Name = name;
            Reference = uniqueName.ToLower().Replace(" ", "_");
        }
    }
}
