using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuButtonData : UIGeneratorTypeTemplate
    {
        public string Name;
        public string Reference;

        [Space]
        public Texture2D Texture;

        [Space]
        public UnityEvent Event;
        public UnityEvent AltEvent;

        public void InvokeEvent() =>
            Event?.Invoke();

        public void InvokeAltEvent() =>
            AltEvent?.Invoke();

        public UIMenuButtonData SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            Name = name;
            Reference = name.ToLower().Replace(" ", "_");
            return this;
        }
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