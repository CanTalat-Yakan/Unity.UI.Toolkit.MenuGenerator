using UnityEngine;

namespace UnityEssentials
{
    public class MenuToggleData : MenuTypeDataBase
    {
        [Space]
        public bool Default;

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset() =>
            Default = false;
    }
}