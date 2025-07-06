using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorSliderData : UIGeneratorTypeTemplate
    {
        [Space]
        public Gradient Gradient;
        [Space]
        [Range(0, 100)]
        public float Default;

        public override void ProfileAddDefault(UIMenuDataProfile profile) =>
            profile.ColorSliders.Add(Reference, Default);

        public override void ApplyDynamicReset() { }
    }
}