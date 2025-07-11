using UnityEngine;
using UnityEngine.Events;

namespace UnityEssentials
{
    public class UIMenuButtonDataConfigurator : MonoBehaviour
    {
        public UIMenuData MenuData;
        public string Reference;

        [Space]
        public UnityEvent Event;
        public UnityEvent AltEvent;

        private UIMenuButtonData _buttonData;
        [HideInInspector] public UIMenuButtonData ButtonData => _buttonData;

        public void Awake()
        {
            if (MenuData?.GetDataByReference(Reference, out _buttonData) ?? false)
            {
                ButtonData.IsDynamic = true;
                ButtonData.Event = Event;
                ButtonData.AltEvent = AltEvent;
            }
        }
    }
}