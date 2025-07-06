using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorPickerData : UIGeneratorTypeTemplate
    {
        [Space]
        public bool HasAlpha;

        [Space]
        public Color Default;

        public override void ProfileAddDefault(UIMenuDataProfile profile) =>
            profile.ColorPickers.Add(Reference, Default);

        public override void ApplyDynamicReset() { }
    }
}