using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputData : UIGeneratorTypeTemplate
    {
        [Space]
        public string Default;

        public override object GetDefault() => Default;
    }
}