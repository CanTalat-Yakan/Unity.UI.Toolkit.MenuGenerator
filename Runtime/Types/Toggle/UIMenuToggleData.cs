using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuToggleData : UIMenuTypeDataBase
    {
        [Space]
        public bool Default;

        public override object GetDefault() => Default;
    }
}