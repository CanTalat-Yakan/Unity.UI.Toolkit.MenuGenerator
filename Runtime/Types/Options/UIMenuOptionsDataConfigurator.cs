using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuOptionsDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuOptionsData>
    {
        [Space]
        public bool Reverse;
        public string[] Options;

        [Space]
        public int Default;

        public override void ApplyDynamicConfiguration()
        {
            Data.Reverse = Reverse;
            Data.Options = Options;
            Data.Default = Default;
        }
    }
}