using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuSelectionCategoryDataGenerator : MenuTypeDataGeneratorBase<MenuSelectionCategoryData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuSelectionCategoryData typedData)
                    using (var generator = new MenuSelectionCategoryDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "SelectionCategory_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuSelectionCategoryData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuSelectionCategoryData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name;

            var image = element.Q<VisualElement>("Image");
            var label = element.Q<Label>("Label");

            var index = menu.Profile.Get<int>(data);

            var selectionData = data.GetSelection(index);
            if (selectionData == null)
                return;

            image.SetBackgroundImage(selectionData.Texture);
            label.text = selectionData.Name;
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuSelectionCategoryData data)
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
                            case MenuColorSliderData sliderData:
                                using (var colorSliderGenerator = new MenuColorSliderDataGenerator())
                                    elements.Add(colorSliderGenerator.CreateElement(menu, sliderData));
                                break;
                            case MenuColorPickerData pickerData:
                                using (var colorPickerGenerator = new MenuColorPickerDataGenerator())
                                    elements.Add(colorPickerGenerator.CreateElement(menu, pickerData));
                                break;
                            case MenuSelectionGroupData group:
                                using (var selectionGenerator = new MenuSelectionDataGenerator())
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