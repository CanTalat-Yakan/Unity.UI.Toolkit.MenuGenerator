using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSliderData : UIGeneratorTypeTemplate
    {
        [Space]
        public bool IsFloat;
        public float MinRange = 0;
        public float MaxRange = 100;

        [Space]
        public float Default;

        public override void ProfileAddDefault(UIMenuDataProfile profile) =>
            profile.Sliders.Add(Reference, Mathf.Clamp(Default, MinRange, MaxRange));

        public override void ApplyDynamicReset() { }
    }
}