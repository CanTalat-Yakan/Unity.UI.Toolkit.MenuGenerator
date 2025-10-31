using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials
{
    public class MenuButtonData : MenuTypeDataBase
    {
        [Space]
        public Texture2D Texture;

        [Space]
        public UnityEvent Event;
        public UnityEvent AltEvent;

        public void InvokeEvent() =>
            Event?.Invoke();

        public void InvokeAltEvent() =>
            AltEvent?.Invoke();

        public override object GetDefault() => null;

        public override void ApplyDynamicReset()
        {
            Event = null;
            AltEvent = null;
        }
    }
}