using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuData : ScriptableObject
    {
        [HideInInspector, SerializeField] public string Name = "Menu";
        [HideInInspector, SerializeField] public ScriptableObject[] Root = { };

        public bool GetDataByReference<T>(string reference, out T data) where T : ScriptableObject
        {
            data = null;
            foreach (var item in GetDataItems())
                if (item is UIGeneratorTypeTemplate dataTemplate)
                    if (dataTemplate.Reference == reference)
                        if (item is T typedData)
                        {
                            data = typedData;
                            return true;
                        }

            return false;
        }

        public IEnumerable<ScriptableObject> GetDataItems()
        {
            foreach (var item in Root)
            {
                yield return item;

                if (item is ScriptableObject scriptableObject)
                    foreach (var child in IterateData(scriptableObject))
                        yield return child;
            }
        }

        public IEnumerable<ScriptableObject> IterateData(ScriptableObject data)
        {
            var field = data.GetType().GetField("Data");
            var children = field?.GetValue(data) as ScriptableObject[];
            if (children != null && children.Length > 0)
                foreach (var child in children)
                    yield return child;
        }
    }
}
