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
    }
}