using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSelectionCategory(
            UIMenuDataGenerator menu,
            UIMenuSelectionCategoryData category)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + "SelectionCategory_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureSelectionCategoryVisuals(menu.Profile, element, category);
            ConfigureSelectionCategoryInteraction(menu, element, category);
            return element;
        }

        private static void ConfigureSelectionCategoryVisuals(
            UIMenuDataProfile profile,
            VisualElement categoryElement,
            UIMenuSelectionCategoryData category)
        {
            var button = categoryElement.Q<Button>("Button");
            button.text = category.Name.ToUpper();

            var image = categoryElement.Q<VisualElement>("Image");
            var label = categoryElement.Q<Label>("Label");

            var index = profile.GetData(category.Reference, category.Default);

            var selectionData = category.GetSelection(index);
            if (selectionData == null)
                return;

            image.SetBackgroundImage(selectionData.Texture);
            label.text = selectionData.Name;
        }

        private static void ConfigureSelectionCategoryInteraction(
            UIMenuDataGenerator menu,
            VisualElement categoryElement,
            UIMenuSelectionCategoryData category)
        {
            var button = categoryElement.Q<Button>();
            button.clicked += () =>
            {
                menu.Populate(false, category.Name, null, () =>
                {
                    foreach (var element in CreateSelectionTiles(menu, categoryElement, category))
                        menu.AddElementToScrollView(element);
                });
            };
        }
    }
}