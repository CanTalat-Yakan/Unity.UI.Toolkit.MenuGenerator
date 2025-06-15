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

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddSelectionCategory(SelectionDataCollectionGroup group)
        {
            var element = CreateSelectionCategory(group);

            AddElementToScrollView(element);
        }
        
        private VisualElement CreateSelectionCategory(SelectionDataCollectionGroup group)
        {
            var element = UIGeneratorData.SelectionCategoryTemplate.CloneTree();

            ConfigureSelectionCategoryVisuals(element, group);
            ConfigureSelectionCategoryInteraction(element, group);

            return element;
        }

        private void ConfigureSelectionCategoryVisuals(VisualElement element, SelectionDataCollectionGroup group)
        {
            var button = element.Q<Button>("Button");
            button.text = group.Name.ToUpper();

            var image = element.Q<VisualElement>("Image");
            var label = element.Q<Label>("Label");

            //if (Profile.SelectionDataDictionary.TryGetValue(group.Reference, out int index))
            //{
            //    var selectionData = group.GetSelectionData(index);
            //    if (selectionData == null)
            //        return;

            //    image.SetImage(selectionData.Texture);
            //    label.text = selectionData.Name;
            //}
        }

        private void ConfigureSelectionCategoryInteraction(VisualElement element, SelectionDataCollectionGroup group)
        {
            var button = element.Q<Button>();
            button.clicked += () => ShowSelectionOverlay(group, UpdateSelectionVisuals(element, group.Reference));
        }
    }

    // Overlay Management - Selection
    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void ShowSelectionOverlay(SelectionDataCollectionGroup group, Action<SelectionData> callback)
        {
            var overlay = CreateOverlay(group.Name);

            foreach (var colorData in group.Colors)
                switch (colorData)
                {
                    case ColorSliderData colorSliderData:
                        overlay.Q<GroupBox>("GroupBox").Add(CreateColorSlider(colorSliderData));
                        break;
                    case ColorPickerData colorPickerData:
                        overlay.Q<GroupBox>("GroupBox").Add(CreateColorPickerButton(colorPickerData.Name, colorPickerData.Reference));
                        break;
                    default: break;
                }

            var content = PopulateSelectionContent(group.Collections, callback);
            foreach (var item in content)
                overlay.Q<GroupBox>("GroupBox").Add(item);

            overlay.Add(CreateSpacer(0));

            _root.LinkedElement.Add(overlay);
        }

        private List<VisualElement> PopulateSelectionContent(ScriptableObject[] collection, Action<SelectionData> callback)
        {
            var content = new List<VisualElement>();
            foreach (var data in collection)
            {
                if (data is SelectionDataCollection collectionData)
                    foreach (var item in collectionData.Data)
                        content.Add(CreateSelectionTile(item, callback));
                else if (data is SelectionData selectionData)
                    content.Add(CreateSelectionTile(selectionData, callback));
            }

            return content;
        }

        private VisualElement CreateSelectionTile(SelectionData data, Action<SelectionData> callback)
        {
            var tile = UIGeneratorData.SelectionTileTemplate.CloneTree();
            tile.Q<Label>().text = data.Name;
            tile.Q<VisualElement>("Image").SetBackgroundImage(data.Texture);
            tile.Q<Button>().clicked += () => callback(data);

            return tile;
        }

        private Action<SelectionData> UpdateSelectionVisuals(VisualElement element, string reference) =>
            selectionData =>
            {
                element.Q<VisualElement>("Image").SetBackgroundImage(selectionData.Texture);
                element.Q<Label>().text = selectionData.Name;

                //Profile.OnSelectionChange(reference, selectionData.ID);
            };
    }
}