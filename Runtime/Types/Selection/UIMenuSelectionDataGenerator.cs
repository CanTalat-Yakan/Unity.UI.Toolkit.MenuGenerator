using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSelectionGeneratorData : IUIMenuTypeData
    {
        public UIMenuSelectionCategoryData CategoryData;
        public UIMenuSelectionDataElement SelectionDataElement;

        [Space]
        public int Index;
    }

    public class UIMenuSelectionDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuSelectionGeneratorData>, IDisposable
    {
        public IEnumerable<VisualElement> CreateElements(
            UIMenuGenerator menu,
            UIMenuSelectionCategoryData categoryData,
            UIMenuSelectionGroupData groupData)
        {
            var selections = groupData.GetSelections();
            for (int i = 0; i < selections.Data.Length; i++)
            {
                var selectionDataElement = selections.Data[i];
                if (selectionDataElement == null)
                    continue;

                var selectionGeneratorData = new UIMenuSelectionGeneratorData
                {
                    CategoryData = categoryData,
                    SelectionDataElement = selectionDataElement,
                    Index = selections.StartIndexID + i
                };

                yield return CreateElement(menu, selectionGeneratorData);
            }
        }

        public static readonly string ResourcePath = Path + "SelectionTile_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuSelectionGeneratorData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuSelectionGeneratorData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.SelectionDataElement.Name;

            var image = element.Q<VisualElement>("Image");
            image.SetBackgroundImage(data.SelectionDataElement.Texture);
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuSelectionGeneratorData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.Profile.OnSelectionValueChanged(data.CategoryData.Reference, data.Index);
        }

        public void Dispose() { }
    }
}
