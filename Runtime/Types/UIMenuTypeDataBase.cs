using System;
using UnityEngine;

namespace UnityEssentials
{
    public interface IUIMenuTypeData { }

    public abstract class UIMenuTypeDataBase : ScriptableObject, IUIMenuTypeData
    {
        [HideInInspector] public bool IsDynamic;
        [HideInInspector] public bool HasReference;
        [HideInInspector] public string ID;
        [HideInInspector] public string Name;
        public string Reference;

        public static T Initialize<T>(string name = null, string uniqueName = null, bool hasReference = true) where T : UIMenuTypeDataBase
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

        public abstract object GetDefault();
        public abstract void ApplyDynamicReset();
    }
}