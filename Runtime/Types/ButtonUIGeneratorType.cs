using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Button_", menuName = "UI/Button", order = 2)]
    public class UIMenuButtonData : ScriptableObject
    {
        public string Name;

        public Texture2D Texture;

        [Space]
        public UnityEvent Event;
        public UnityEvent AltEvent;

        public void InvokeEvent() =>
            Event?.Invoke();

        public void InvokeAltEvent() =>
            AltEvent?.Invoke();
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateButton(UIMenuGenerator menu, UIMenuButtonData data)
        {
            var element = menu.Data.ButtonTemplate.CloneTree();

            ConfigureButtonVisuals(element, data);
            ConfigureButtonInteraction(element, data);

            return element;
        }

        private static void ConfigureButtonVisuals(VisualElement element, UIMenuButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        private static void ConfigureButtonInteraction(VisualElement element, UIMenuButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => data.InvokeEvent();
        }
    }
}