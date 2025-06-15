using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials
{
    public class ButtonDataEvent : MonoBehaviour
    {
        [SerializeField] private ButtonData Data;
        [SerializeField] private UnityEvent Event;
        [SerializeField] private UnityEvent AltEvent;

        public void OnEnable()
        {
            if (Event != null)
                Data?.Event.AddListener(Event.Invoke);

            if (AltEvent != null)
                Data?.AltEvent.AddListener(AltEvent.Invoke);
        }

        public void OnDisable()
        {
            if (Event != null)
                Data?.Event.RemoveListener(Event.Invoke);

            if (AltEvent != null)
                Data?.AltEvent.RemoveListener(AltEvent.Invoke);
        }
    }
}