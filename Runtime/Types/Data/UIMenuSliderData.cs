using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSliderData : UIMenuGeneratorTypeTemplate
    {
        [Space]
        public bool IsFloat;
        public float MinRange = 0;
        public float MaxRange = 100;

        [Space]
        public float Default;

        public override object GetDefault() => Mathf.Clamp(Default, MinRange, MaxRange);
    }
}