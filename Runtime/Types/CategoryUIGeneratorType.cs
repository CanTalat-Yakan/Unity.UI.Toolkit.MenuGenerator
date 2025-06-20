using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Category_", menuName = "UI/Category", order = 1)]
    public class CategoryData : ScriptableObject
    {
        public string Name;

        [Space]
        public Texture2D Texture;
        public ScriptableObject[] Data;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateCategory(UIMenuGenerator menu, CategoryData data)
        {
            var element = menu.UIGeneratorData.CategoryTemplate.CloneTree();

            ConfigureCategoryVisuals(element, data);
            ConfigureCategoryInteraction(element, data, menu);

            return element;
        }

        private static void ConfigureCategoryVisuals(VisualElement element, CategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        private static void ConfigureCategoryInteraction(VisualElement element, CategoryData data, UIMenuGenerator menu)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.PopulateHierarchy(data.Name, true, data.Data);
        }
    }
}