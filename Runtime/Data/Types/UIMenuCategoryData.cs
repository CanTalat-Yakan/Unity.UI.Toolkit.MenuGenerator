using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuCategoryData : UIGeneratorTypeTemplate
    {
        public Texture2D Texture;

        public ScriptableObject[] Data;

        public override void ProfileAddDefault(UIMenuDataProfile profile) { }

        public override void ApplyDynamicReset() { }
    }
}