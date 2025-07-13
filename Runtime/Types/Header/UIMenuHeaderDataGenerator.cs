using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateHeader(UIMenuDataGenerator menu, UIMenuHeaderData data)
        {
            var path = "UIToolkit/UXML/Templates_Default_UI_";
            var name = path + "Header_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureHeaderVisuals(element, data);
            return element;
        }

        private static void ConfigureHeaderVisuals(VisualElement element, UIMenuHeaderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();
            label.style.marginTop = data.MarginTop;
        }
    }
}