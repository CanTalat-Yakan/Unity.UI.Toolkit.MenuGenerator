using UnityEngine;

namespace UnityEssentials
{
    public class MenuColorSliderData : MenuTypeDataBase
    {
        [Space]
        public Gradient Gradient;
        [Space]
        [Range(0, 100)]
        public float Default;

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset()
        {
            Gradient = null;
            Default = 0f;
        }
    }
}