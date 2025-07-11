using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuToggleData : UIMenuGeneratorTypeTemplate
    {
        [Space]
        public bool Default;

        public override object GetDefault() => Default;
    }
}