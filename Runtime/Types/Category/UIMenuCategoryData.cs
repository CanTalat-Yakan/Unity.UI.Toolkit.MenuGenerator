using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuCategoryData : UIMenuTypeDataBase
    {
        public Texture2D Texture;

        public ScriptableObject[] Data;

        public override object GetDefault() => null;

        public override void ApplyDynamicReset() { }
    }
}