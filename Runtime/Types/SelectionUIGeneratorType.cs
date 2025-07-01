using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSelectionDataCategory : UIGeneratorTypeTemplate
    {
        [Space]
        public ScriptableObject[] Data;

        [Space]
        public int Default;

        public UIMenuSelectionDataElement GetSelectionData(int index)
        {
            foreach (var scriptableObject in Data)
                if (scriptableObject is UIMenuSelectionDataGroup group)
                    foreach (var selections in group.Selections)
                        for (int i = 0; i < selections.Data.Length; i++)
                            if (selections.ID + i == index)
                                return selections.Data[i];

            return null;
        }
    }

    public class UIMenuSelectionDataGroup : UIGeneratorTypeTemplate
    {
        public UIMenuSelectionData[] Selections;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSelectionCategory(
            UIMenuGenerator menu,
            UIMenuSelectionDataCategory category)
        {
            var categoryElement = menu.Data.SelectionCategoryTemplate.CloneTree();

            ConfigureSelectionCategoryVisuals(menu.Profile, categoryElement, category);
            ConfigureSelectionCategoryInteraction(menu, categoryElement, category);

            return categoryElement;
        }

        private static void ConfigureSelectionCategoryVisuals(
            UIMenuDataProfile profile,
            VisualElement categoryElement,
            UIMenuSelectionDataCategory category)
        {
            var button = categoryElement.Q<Button>("Button");
            button.text = category.Name.ToUpper();

            var image = categoryElement.Q<VisualElement>("Image");
            var label = categoryElement.Q<Label>("Label");

            if (!profile.SelectionDataDictionary.TryGetValue(category.Reference, out var index))
                index = category.Default;

            var selectionData = category.GetSelectionData(index);
            if (selectionData == null)
                return;

            image.SetBackgroundImage(selectionData.Texture);
            label.text = selectionData.Name;
        }

        private static void ConfigureSelectionCategoryInteraction(
            UIMenuGenerator menu,
            VisualElement categoryElement,
            UIMenuSelectionDataCategory category)
        {
            var button = categoryElement.Q<Button>();
            button.clicked += () =>
            {
                menu.PopulateHierarchy(false, category.Name, null);

                foreach (var element in AddSelectionTiles(menu, categoryElement, category))
                    menu.AddElementToScrollView(element);
            };
        }
    }

    // Overlay Management - Selection
    public static partial class UIMenuGeneratorType
    {
        public static IEnumerable<VisualElement> AddSelectionTiles(
            UIMenuGenerator menu,
            VisualElement categoryElement,
            UIMenuSelectionDataCategory category)
        {
            foreach (var scriptableObject in category.Data)
                switch (scriptableObject)
                {
                    case UIMenuColorSliderData sliderData:
                        yield return CreateColorSlider(menu, sliderData);
                        break;
                    case UIMenuColorPickerData pickerData:
                        yield return CreateColorPickerButton(menu, pickerData.Name, pickerData.Reference);
                        break;
                    case UIMenuSelectionDataGroup group:
                        yield return CreateGroupBoxSelectionTiles(group, menu, categoryElement, category);
                        break;
                }
        }

        private static VisualElement CreateGroupBoxSelectionTiles(
            UIMenuSelectionDataGroup group,
            UIMenuGenerator menu,
            VisualElement categoryElement,
            UIMenuSelectionDataCategory category)
        {
            var groupBox = new GroupBox();
            groupBox.SetWidth(100f);
            groupBox.style.flexWrap = Wrap.Wrap;
            foreach (var selections in group.Selections)
                for (int i = 0; i < selections.Data.Length; i++)
                    groupBox.Add(CreateSelectionTile(
                        menu, categoryElement, category, selections.Data[i], selections.ID + i));
            return groupBox;
        }

        private static VisualElement CreateSelectionTile(
            UIMenuGenerator menu,
            VisualElement categorycategoryElement,
            UIMenuSelectionDataCategory category,
            UIMenuSelectionDataElement data,
            int index)
        {
            var tile = menu.Data.SelectionTileTemplate.CloneTree();
            tile.Q<Label>().text = data.Name;
            tile.Q<VisualElement>("Image").SetBackgroundImage(data.Texture);
            tile.Q<Button>().clicked += () => UpdateSelectionVisuals(menu.Profile, categorycategoryElement, data, category.Reference, index);

            return tile;
        }

        private static void UpdateSelectionVisuals(
            UIMenuDataProfile profile,
            VisualElement categorycategoryElement,
            UIMenuSelectionDataElement data,
            string reference,
            int index)
        {
            categorycategoryElement.Q<VisualElement>("Image").SetBackgroundImage(data.Texture);
            categorycategoryElement.Q<Label>().text = data.Name;
        }
    }
}