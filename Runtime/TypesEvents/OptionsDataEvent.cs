using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials
{
    public class OptionsDataEvent : MonoBehaviour
    {
        [SerializeField] private OptionsData Data;
        [SerializeField] private UnityEvent<OptionsData> Event;

        public void OnEnable() =>
            Event?.Invoke(Data);
    }
}