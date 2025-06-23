using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Category_", menuName = "UI/Category", order = 1)]
    public class UIMenuCategoryData : ScriptableObject
    {
        public string Name;

        [Space]
        public Texture2D Texture;
        public ScriptableObject[] Data;
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