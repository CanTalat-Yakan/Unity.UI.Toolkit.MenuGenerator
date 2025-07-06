using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuToggleData : UIGeneratorTypeTemplate
    {
        [Space]
        public bool Default;

        public override void ProfileAddDefault(UIMenuDataProfile profile) =>
            profile.Toggles.Add(Reference, Default);

        public override void ApplyDynamicReset() { }
    }
}