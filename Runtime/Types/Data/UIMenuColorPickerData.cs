using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorPickerData : UIMenuGeneratorTypeTemplate
    {
        [Space]
        public bool HasAlpha;

        [Space]
        public Color Default;

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset()
        {
            Default = Color.white;
            HasAlpha = false;
        }
    }
}