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

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddCategory(CategoryData data)
        {
            var element = CreateCategory(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateCategory(CategoryData data)
        {
            var element = UIGeneratorData.CategoryTemplate.CloneTree();

            ConfigureCategoryVisuals(element, data);
            ConfigureCategoryInteraction(element, data);

            return element;
        }

        private void ConfigureCategoryVisuals(VisualElement element, CategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        private void ConfigureCategoryInteraction(VisualElement element, CategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => PopulateHierarchy(data.Name, true, data.Data);
        }
    }
}