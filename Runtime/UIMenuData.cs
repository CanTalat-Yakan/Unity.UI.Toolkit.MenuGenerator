using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuData : ScriptableObject
    {
        [HideInInspector] public string Name = "Menu";
        [HideInInspector] public ScriptableObject[] Root = { };

        public bool TryGetValue<T>(string dataReference, out T outputData) where T : ScriptableObject
        {
            outputData = null;
            foreach (var data in EnumerateAllData())
                if (data is UIMenuTypeDataBase dataTemplate)
                    if (dataTemplate.Reference == dataReference)
                        if (data is T typedData)
                        {
                            outputData = typedData;
                            return true;
                        }

            return false;
        }

        public IEnumerable<ScriptableObject> EnumerateAllData()
        {
            foreach (var data in Root)
            {
                yield return data;

                if (data is ScriptableObject scriptableObject)
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
