using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputData : UIMenuGeneratorTypeTemplate
    {
        [Space]
        public string Default;

        public override object GetDefault() => Default;
    }
}