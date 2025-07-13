using System;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuTypeBase : ScriptableObject
    {
        [HideInInspector] public bool IsDynamic;
        [HideInInspector] public bool HasReference;
        [HideInInspector] public string ID;
        [HideInInspector] public string Name;
        public string Reference;

        public static T Initialize<T>(string name = null, string uniqueName = null, bool hasReference = true) where T : UIMenuTypeBase
        {
            var generatorType = CreateInstance<T>();
            generatorType.HasReference = hasReference;
            generatorType.ID = Guid.NewGuid().ToString();
            generatorType.SetName(name ?? string.Empty, uniqueName);
            return generatorType;
        }

        public void SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            Name = name;
            Reference = uniqueName.ToLower().Replace(" ", "_");

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        public virtual object GetDefault() => null;
        public virtual void ApplyDynamicReset() { }
    }
}
