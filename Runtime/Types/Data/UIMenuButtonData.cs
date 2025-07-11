using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials
{
    public class UIMenuButtonData : UIGeneratorTypeTemplate
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
    }
}