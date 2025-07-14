using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSelectionCategoryDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuSelectionCategoryData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuSelectionCategoryData typedData)
                    using (var generator = new UIMenuSelectionCategoryDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "SelectionCategory_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuSelectionCategoryData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuSelectionCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            var image = element.Q<VisualElement>("Image");
            var label = element.Q<Label>("Label");

            var index = menu.Profile.GetData(data.Reference, data.Default);

            var selectionData = data.GetSelection(index);
            if (selectionData == null)
                return;

            image.SetBackgroundImage(selectionData.Texture);
            label.text = selectionData.Name;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuSelectionCategoryData data)
        {
            var button = element.Q<Button>();
            button.clicked += () =>
            {
                menu.Populate(false, data.Name, null, () =>
                {
                    var elements = new List<VisualElement>();
                    foreach (var scriptableObject in data.Data)
                        switch (scriptableObject)
                        {
                            case UIMenuColorSliderData sliderData:
                                using (var colorSliderGenerator = new UIMenuColorSliderDataGenerator())
                                    elements.Add(colorSliderGenerator.CreateElement(menu, sliderData));
                                break;
                            case UIMenuColorPickerData pickerData:
                                using (var colorPickerGenerator = new UIMenuColorPickerDataGenerator())
                                    elements.Add(colorPickerGenerator.CreateElement(menu, pickerData));
                                break;
                            case UIMenuSelectionGroupData group:
                                using (var selectionGenerator = new UIMenuSelectionDataGenerator())
                                {
                                    var selectionElements = selectionGenerator.CreateElements(menu, data, group);
                                    elements.Add(WrapInGroupBox(selectionElements));
                                }
                                break;
                        }

                    foreach (var element in elements)
                        menu.AddToScrollView(element);
                });
            };
        }

        private static VisualElement WrapInGroupBox(IEnumerable<VisualElement> elements)
        {
            var groupBox = new GroupBox();
            groupBox.SetWidth(100f);
            groupBox.style.flexWrap = Wrap.Wrap;

            foreach (var element in elements)
                groupBox.Add(element);

            return groupBox;
        }

        public void Dispose() { }
    }
}