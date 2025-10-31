using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials
{
    public class MenuButtonDataConfigurator : MenuTypeDataConfiguratorBase<MenuButtonData>
    {
        [Space]
        public UnityEvent Event;
        public UnityEvent AltEvent;

        public override void ApplyDynamicConfiguration()
        {
            Data.Event = Event;
            Data.AltEvent = AltEvent;
        }
    }
}