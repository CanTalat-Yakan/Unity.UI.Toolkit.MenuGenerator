using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuToggleData : UIMenuTypeBase
    {
        [Space]
        public bool Default;

        public override object GetDefault() => Default;
    }
}