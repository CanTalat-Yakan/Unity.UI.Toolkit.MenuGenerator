using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateButton(UIMenuDataGenerator menu, UIMenuButtonData data)
        {
            var path = "UIToolkit/UXML/Templates_Default_UI_";
            var name = path + "Button_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
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