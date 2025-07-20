using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuOptionsDataConfigurator : UIMenuTypeDataConfiguratorBase<UIMenuOptionsData>
    {
        [Space]
        public string[] Options;

        [Space]
        public int Default;

        public override void ApplyDynamicConfiguration()
        {
            Data.Options = Options;
            Data.Default = Default;
        }
    }
}