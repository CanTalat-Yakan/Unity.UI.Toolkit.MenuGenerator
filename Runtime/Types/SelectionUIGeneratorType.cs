using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSelectionDataCategory : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public ScriptableObject[] Data;

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

        public UIMenuSelectionDataCategory SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            Name = name;
            Reference = name.ToLower().Replace(" ", "_");
            return this;
        }
    }

    public class UIMenuSelectionDataGroup : ScriptableObject
    {
        public string Name;

        public UIMenuSelectionData[] Selections;

        public UIMenuSelectionDataGroup SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            base.name = name;
            Name = name;
            return this;
        }
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

            if (profile.SelectionDataDictionary.TryGetValue(category.Reference, out int index))
            {
                var selectionData = category.GetSelectionData(index);
                if (selectionData == null)
                    return;

                image.SetBackgroundImage(selectionData.Texture);
                label.text = selectionData.Name;
            }
        }

        private static void ConfigureSelectionCategoryInteraction(
            UIMenuGenerator menu, 
            VisualElement categoryElement, 
            UIMenuSelectionDataCategory category)
        {
            var button = categoryElement.Q<Button>();
            button.clicked += () =>
                ShowSelectionOverlay(menu, categoryElement, category);
        }
    }

    // Overlay Management - Selection
    public static partial class UIMenuGeneratorType
    {
        private static void ShowSelectionOverlay(
            UIMenuGenerator menu, 
            VisualElement categoryElement, 
            UIMenuSelectionDataCategory category)
        {
            var overlay = menu.CreatePopup(category.Name);
            var groupBox = overlay.Q<GroupBox>("GroupBox");

            foreach (var scriptableObject in category.Data)
                switch (scriptableObject)
                {
                    case UIMenuColorSliderData sliderData:
                        groupBox.Add(CreateColorSlider(menu, sliderData));
                        break;
                    case UIMenuColorPickerData pickerData:
                        groupBox.Add(CreateColorPickerButton(menu, pickerData.Name, pickerData.Reference));
                        break;
                    case UIMenuSelectionDataGroup group:
                        AddSelectionTiles(menu, groupBox, categoryElement, category, group);
                        break;
                }

            menu.AddElementToRoot(overlay);
        }

        private static void AddSelectionTiles(
            UIMenuGenerator menu,
            VisualElement groupBox,
            VisualElement categoryElement,
            UIMenuSelectionDataCategory category,
            UIMenuSelectionDataGroup group)
        {
            foreach (var selections in group.Selections)
                for (int i = 0; i < selections.Data.Length; i++)
                {
                    groupBox.Add(CreateSelectionTile(
                        menu, categoryElement, category, selections.Data[i], selections.ID + i));
                }
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