using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputData : UIMenuTypeDataBase
    {
        [Space]
        public string Default;

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset() =>
            Default = string.Empty;
    }
}