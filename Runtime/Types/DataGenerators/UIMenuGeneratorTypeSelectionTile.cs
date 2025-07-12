using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        private static VisualElement CreateSelectionTile(
            UIMenuDataGenerator menu,
            VisualElement categoryElement,
            UIMenuSelectionCategoryData categoryData,
            UIMenuSelectionDataElement elementData,
            int index)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + "SelectionTile_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            element.Q<Label>().text = elementData.Name;
            element.Q<VisualElement>("Image").SetBackgroundImage(elementData.Texture);
            element.Q<Button>().clicked += () => UpdateSelectionTileVisuals(
                menu.Profile, categoryElement, elementData, categoryData.Reference, index);

            return element;
        }

        private static void UpdateSelectionTileVisuals(
            UIMenuDataProfile profile,
            VisualElement categoryElement,
            UIMenuSelectionDataElement elementData,
            string reference,
            int index)
        {
            profile.OnSelectionValueChanged(reference, index);

            categoryElement.Q<VisualElement>("Image").SetBackgroundImage(elementData.Texture);
            categoryElement.Q<Label>().text = elementData.Name;
        }

        public static IEnumerable<VisualElement> CreateSelectionTiles(
            UIMenuDataGenerator menu,
            VisualElement categoryElement,
            UIMenuSelectionCategoryData categoryData)
        {
            foreach (var scriptableObject in categoryData.Data)
                switch (scriptableObject)
                {
                    case UIMenuColorSliderData sliderData:
                        yield return CreateColorSlider(menu, sliderData);
                        break;
                    case UIMenuColorPickerData pickerData:
                        yield return CreateColorPickerButton(menu, pickerData.Name, pickerData.Reference);
                        break;
                    case UIMenuSelectionGroupData group:
                        yield return CreateGroupBoxSelectionTiles(group, menu, categoryElement, categoryData);
                        break;
                }
        }

        private static VisualElement CreateGroupBoxSelectionTiles(
            UIMenuSelectionGroupData group,
            UIMenuDataGenerator menu,
            VisualElement categoryElement,
            UIMenuSelectionCategoryData categoryData)
        {
            var groupBox = new GroupBox();
            groupBox.SetWidth(100f);
            groupBox.style.flexWrap = Wrap.Wrap;

            var selectionsList = group.GetSelections();
            if (selectionsList == null)
                return groupBox;

            foreach (var selections in selectionsList)
            {
                if (selections == null || selections.Data == null)
                    continue;

                for (int i = 0; i < selections.Data.Length; i++)
                {
                    if (selections.Data[i] == null)
                        continue;

                    groupBox.Add(CreateSelectionTile(
                        menu, categoryElement, categoryData, selections.Data[i], selections.StartIndexID + i));
                }
            }

            return groupBox;
        }
    }
}
