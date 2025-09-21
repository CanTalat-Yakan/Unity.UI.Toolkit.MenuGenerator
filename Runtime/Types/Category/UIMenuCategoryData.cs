using System.Collections.Generic;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuCategoryData : UIMenuTypeDataBase
    {
        [Space]
        public Texture2D Texture;

        public ScriptableObject[] Data;

        public void AddData(params ScriptableObject[] data)
        {
            var dataList = new List<ScriptableObject>(Data);
            dataList.AddRange(data);
            Data = dataList.ToArray();
        }

        public override object GetDefault() => null;

        public override void ApplyDynamicReset() =>
            Data = null;
    }
}