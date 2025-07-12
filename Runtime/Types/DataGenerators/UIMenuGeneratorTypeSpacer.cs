using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSpacer(UIMenuDataGenerator menu, int height)
        {
            var data = ScriptableObject.CreateInstance<UIMenuSpacerData>();
            data.Height = height;

            return CreateSpacer(menu, data);
        }

        public static VisualElement CreateSpacer(UIMenuDataGenerator menu, UIMenuSpacerData data)
        {
            var path = "UIToolkit/UXML/Templates_Default_UI_";
            var name = path + "Spacer_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureOptionsVisuals(element, data);
            return element;
        }

        private static void ConfigureOptionsVisuals(VisualElement element, UIMenuSpacerData data)
        {
            var spacer = element.Q<VisualElement>("Spacer");
            spacer.SetHeight(data.Height);
        }
    }
}