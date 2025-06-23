using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials
{
    public class OptionsDataEvent : MonoBehaviour
    {
        [SerializeField] private UIMenuOptionsData Data;
        [SerializeField] private UnityEvent<UIMenuOptionsData> Event;

        public void OnEnable() =>
            Event?.Invoke(Data);
    }
}