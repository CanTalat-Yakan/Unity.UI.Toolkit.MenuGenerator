using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSliderData : UIMenuTypeDataBase
    {
        [Space]
        public bool IsFloat;
        public float MinValue = 0;
        public float MaxValue = 100;

        [Space]
        public float Default;

        public override object GetDefault() => Mathf.Clamp(Default, MinValue, MaxValue);

        public override void ApplyDynamicReset()
        {
            IsFloat = false;
            MinValue = 0;
            MaxValue = 100;
            Default = 0;
        }
    }
}