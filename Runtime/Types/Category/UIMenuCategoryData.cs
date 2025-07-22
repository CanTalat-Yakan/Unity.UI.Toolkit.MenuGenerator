using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuCategoryData : UIMenuTypeDataBase
    {
        [Space]
        public Texture2D Texture;

        public ScriptableObject[] Data;

        public override object GetDefault() => Data;

        public override void ApplyDynamicReset() =>
            Data = null;
    }
}