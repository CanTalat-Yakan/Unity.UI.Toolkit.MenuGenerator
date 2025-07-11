using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuToggleData : UIGeneratorTypeTemplate
    {
        [Space]
        public bool Default;

        public override object GetDefault() => Default;
    }
}