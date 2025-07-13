using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSelectionGeneratorData : UIMenuTypeBase
    {
        public UIMenuSelectionCategoryData CategoryData;
        public UIMenuSelectionDataElement ElementData;

        [Space]
        public int Index;
    }

    public class UIMenuSelectionDataGenerator : UIMenuGeneratorTypeBase<UIMenuSelectionGeneratorData>, IDisposable
    {
        public IEnumerable<VisualElement> CreateElements(
            UIMenuDataGenerator menu,
            UIMenuSelectionCategoryData categoryData,
            UIMenuSelectionGroupData groupData)
        {
            var selections = groupData.GetSelections();
            for (int i = 0; i < selections.Data.Length; i++)
            {
                var selectionElement = selections.Data[i];
                if (selectionElement == null)
                    continue;

                var selectionGeneratorData = new UIMenuSelectionGeneratorData
                {
                    CategoryData = categoryData,
                    ElementData = selectionElement,
                    Index = selections.StartIndexID + i
                };

                yield return CreateElement(menu, selectionGeneratorData);
            }
        }

        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuSelectionGeneratorData data)
        {
            var resourcePath = Path + "SelectionTile_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(resourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuSelectionGeneratorData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.ElementData.Name;

            var image = element.Q<VisualElement>("Image");
            image.SetBackgroundImage(data.ElementData.Texture);
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuSelectionGeneratorData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => menu.Profile.OnSelectionValueChanged(data.CategoryData.Reference, data.Index);
        }

        public void Dispose() { }
    }
}
