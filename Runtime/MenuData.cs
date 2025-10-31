using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    public class MenuData : ScriptableObject
    {
        public string Name = "Menu";
        public ScriptableObject[] Root = { };

        public void AddRoot(params ScriptableObject[] data)
        {
            if (Root == null || Root.Length == 0)
                Root = Array.Empty<ScriptableObject>();
            var rootList = new List<ScriptableObject>(Root);
            rootList.AddRange(data);
            Root = rootList.ToArray();
        }

        public bool TryGetValue<T>(string dataReference, out T outputData) where T : ScriptableObject
        {
            outputData = null;
            foreach (var data in EnumerateAllData())
                if (data is MenuTypeDataBase dataTemplate)
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

                foreach (var child in IterateData(data))
                    yield return child;
            }
        }

        public IEnumerable<ScriptableObject> IterateData(ScriptableObject data)
        {
            var field = data.GetType().GetField("Data");
            if (field?.GetValue(data) is ScriptableObject[] children && children.Length > 0)
                foreach (var child in children)
                    yield return child;
        }
    }
}