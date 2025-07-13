using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorPickerData : UIMenuTypeBase
    {
        [Space]
        public bool HasAlpha;

        [Space]
        public Color Default;

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset()
        {
            Default = Color.black;
            HasAlpha = false;
        }
    }
}