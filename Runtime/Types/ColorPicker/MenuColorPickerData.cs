using UnityEngine;

namespace UnityEssentials
{
    public class MenuColorPickerData : MenuTypeDataBase
    {
        [Space]
        public bool HasAlpha;

        [Space]
        public Color Default;

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset()
        {
            HasAlpha = false;
            Default = default;
        }
    }
}