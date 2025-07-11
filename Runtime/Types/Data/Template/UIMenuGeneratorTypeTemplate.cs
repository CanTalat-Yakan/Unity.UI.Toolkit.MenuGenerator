using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuGeneratorTypeTemplate : ScriptableObject
    {
        [HideInInspector] public bool IsDynamic;
        [HideInInspector] public bool HasReference;
        [HideInInspector] public string ID;
        [HideInInspector] public string Name;
        public string Reference;

        public static T Initialize<T>(string name = null, string uniqueName = null, bool hasReference = true) where T : UIMenuGeneratorTypeTemplate
        {
            var generatorType = CreateInstance<T>();
            generatorType.HasReference = hasReference;
            generatorType.ID = GUID.Generate().ToString();
            generatorType.SetName(name ?? string.Empty, uniqueName);
            return generatorType;
        }

        public void SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            Name = name;
            Reference = uniqueName.ToLower().Replace(" ", "_");

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        public virtual object GetDefault() => null;

        public virtual void ApplyDynamicReset() { }
    }
}
