using UnityEngine;

namespace UnityEssentials
{
    public class MenuOptionsDataConfigurator : MenuTypeDataConfiguratorBase<MenuOptionsData>
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