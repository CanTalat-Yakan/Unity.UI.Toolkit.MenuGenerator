using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateCategory(UIMenuDataGenerator menu, UIMenuCategoryData data)
        {
            var path = "UIToolkit/UXML/Templates_Default_UI_";
            var name = path + "Category_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureCategoryVisuals(element, data);
            ConfigureCategoryInteraction(menu, element, data);
            return element;
        }

        private static void ConfigureCategoryVisuals(VisualElement element, UIMenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        private static void ConfigureCategoryInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.Populate(false, data.Name, data.Data);
        }
    }
}