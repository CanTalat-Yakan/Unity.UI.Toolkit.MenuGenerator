using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSelectionDataCategory : UIGeneratorTypeTemplate
    {
        public ScriptableObject[] Data;

        public int Default;

        public UIMenuSelectionDataElement GetSelection(int index)
        {
            foreach (var scriptableObject in Data)
                if (scriptableObject is UIMenuSelectionDataGroup group)
                    foreach (var selections in group.Selections)
                        if (selections != null && selections.Data != null)
                            for (int i = 0; i < selections.Data.Length; i++)
                                if (selections.StartIndexID + i == index)
                                    return selections.Data[i];

            return null;
        }

        public override void ProfileAddDefault(UIMenuDataProfile profile) =>
            profile.Selections.Add(Reference, Default);
    }

    public class UIMenuSelectionDataGroup : UIGeneratorTypeTemplate
    {
        [Space]
        public UIMenuSelectionData[] Selections;

        public override void ProfileAddDefault(UIMenuDataProfile profile) { }
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

            profile.Selections.TryGetValue(category.Reference, category.Default, out var index);

            var selectionData = category.GetSelection(index);
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
                menu.Populate(false, category.Name, null, () =>
                {
                    foreach (var element in AddSelectionTiles(menu, categoryElement, category))
                        menu.AddElementToScrollView(element);
                });
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

            if (group.Selections == null)
                return groupBox;

            foreach (var selections in group.Selections)
            {
                if (selections == null || selections.Data == null)
                    continue;

                for (int i = 0; i < selections.Data.Length; i++)
                {
                    if (selections.Data[i] == null)
                        continue;

                    groupBox.Add(CreateSelectionTile(
                        menu, categoryElement, category, selections.Data[i], selections.StartIndexID + i));
                }
            }

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
            profile.OnSelectionValueChanged(reference, index);

            categorycategoryElement.Q<VisualElement>("Image").SetBackgroundImage(data.Texture);
            categorycategoryElement.Q<Label>().text = data.Name;
        }
    }
}