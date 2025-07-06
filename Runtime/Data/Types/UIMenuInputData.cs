using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuInputData : UIGeneratorTypeTemplate
    {
        [Space]
        public string Default;

        public override void ProfileAddDefault(UIMenuDataProfile profile) =>
            profile.Inputs.Add(Reference, Default);

        public override void ApplyDynamicReset() { }
    }
}