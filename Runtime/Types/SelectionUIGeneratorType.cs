using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "SelectionData_", menuName = "UI/Data/Selections/Selection", order = 0)]
    public class SelectionData : ScriptableObject
    {
        public string Name;
        public int ID;

        [Space]
        public Texture2D Texture;
    }

    [CreateAssetMenu(fileName = "SelectionDataCollection_", menuName = "UI/Data/Selections/Selection Collection", order = 1)]
    public class SelectionDataCollection : ScriptableObject
    {
        public SelectionData[] Data;
    }

    [CreateAssetMenu(fileName = "SelectionDataCollectionGroup_", menuName = "UI/Data/Selections/Selection Collection Group", order = 2)]
    public class SelectionDataCollectionGroup : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public SelectionDataCollection[] Collections;
        public ScriptableObject[] Colors;

        public SelectionData GetSelectionData(int index)
        {
            foreach (var collection in Collections)
                foreach (var selectionData in collection.Data)
                    if (selectionData.ID == index)
                        return selectionData;

            return null;
        }
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSelectionCategory(UIMenuGenerator menu, SelectionDataCollectionGroup group)
        {
            var element = menu.Data.SelectionCategoryTemplate.CloneTree();

            ConfigureSelectionCategoryVisuals(menu.Profile, element, group);
            ConfigureSelectionCategoryInteraction(menu, element, group);

            return element;
        }

        private static void ConfigureSelectionCategoryVisuals(UIMenuDataProfile profile, VisualElement element, SelectionDataCollectionGroup group)
        {
            var button = element.Q<Button>("Button");
            button.text = group.Name.ToUpper();

            var image = element.Q<VisualElement>("Image");
            var label = element.Q<Label>("Label");

            if (profile.SelectionDataDictionary.TryGetValue(group.Reference, out int index))
            {
                var selectionData = group.GetSelectionData(index);
                if (selectionData == null)
                    return;

                image.SetBackgroundImage(selectionData.Texture);
                label.text = selectionData.Name;
            }
        }

        private static void ConfigureSelectionCategoryInteraction(UIMenuGenerator menu, VisualElement element, SelectionDataCollectionGroup group)
        {
            var button = element.Q<Button>();
            button.clicked += () => 
                ShowSelectionOverlay(menu, group, callback:
                    UpdateSelectionVisuals(menu.Profile, element, group.Reference));
        }
    }

    // Overlay Management - Selection
    public static partial class UIMenuGeneratorType
    {
        private static void ShowSelectionOverlay(UIMenuGenerator menu, SelectionDataCollectionGroup group, Action<SelectionData> callback)
        {
            var overlay = menu.CreatePopup(group.Name);

            foreach (var colorData in group.Colors)
                switch (colorData)
                {
                    case ColorSliderData colorSliderData:
                        overlay.Q<GroupBox>("GroupBox").Add(CreateColorSlider(menu, colorSliderData));
                        break;
                    case ColorPickerData colorPickerData:
                        overlay.Q<GroupBox>("GroupBox").Add(CreateColorPickerButton(menu,colorPickerData.Name, colorPickerData.Reference));
                        break;
                    default: break;
                }

            var content = PopulateSelectionContent(menu, group.Collections, callback);
            foreach (var item in content)
                overlay.Q<GroupBox>("GroupBox").Add(item);

            menu.AddElementToRoot(overlay);
        }

        private static List<VisualElement> PopulateSelectionContent(UIMenuGenerator menu, ScriptableObject[] collection, Action<SelectionData> callback)
        {
            var content = new List<VisualElement>();
            foreach (var data in collection)
            {
                if (data is SelectionDataCollection collectionData)
                    foreach (var item in collectionData.Data)
                        content.Add(CreateSelectionTile(menu, item, callback));
                else if (data is SelectionData selectionData)
                    content.Add(CreateSelectionTile(menu, selectionData, callback));
            }

            return content;
        }

        private static VisualElement CreateSelectionTile(UIMenuGenerator menu, SelectionData data, Action<SelectionData> callback)
        {
            var tile = menu.Data.SelectionTileTemplate.CloneTree();
            tile.Q<Label>().text = data.Name;
            tile.Q<VisualElement>("Image").SetBackgroundImage(data.Texture);
            tile.Q<Button>().clicked += () => callback(data);

            return tile;
        }

        private static Action<SelectionData> UpdateSelectionVisuals(UIMenuDataProfile profile, VisualElement element, string reference) =>
            selectionData =>
            {
                element.Q<VisualElement>("Image").SetBackgroundImage(selectionData.Texture);
                element.Q<Label>().text = selectionData.Name;

                profile.OnSelectionChange(reference, selectionData.ID);
            };
    }
}