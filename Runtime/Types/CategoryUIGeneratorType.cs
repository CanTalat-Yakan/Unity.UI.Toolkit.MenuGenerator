using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuCategoryData : UIGeneratorTypeTemplate
    {
        public string Name;

        public Texture2D Texture;
        public ScriptableObject[] Data;

        public UIMenuCategoryData SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            base.name = uniqueName;
            Name = name;
            return this;
        }
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateCategory(UIMenuGenerator menu, UIMenuCategoryData data)
        {
            var element = menu.Data.CategoryTemplate.CloneTree();

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

        private static void ConfigureCategoryInteraction(UIMenuGenerator menu, VisualElement element, UIMenuCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.PopulateHierarchy(false, data.Name, data.Data);
        }
    }
}