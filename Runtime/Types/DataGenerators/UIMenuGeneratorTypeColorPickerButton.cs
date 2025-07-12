using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateColorPickerButton(UIMenuDataGenerator menu, UIMenuColorPickerData data)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + "ColorPickerButton_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureColorPickerButtonVisuals(menu.Profile, element, data);
            ConfigureColorPickerButtonInteraction(menu, element, data);
            return element;
        }

        private static VisualElement CreateColorPickerButton(UIMenuDataGenerator menu, string name, string reference)
        {
            var data = new UIMenuColorPickerData()
            {
                Name = name,
                Reference = reference,
            };

            return CreateColorPickerButton(menu, data);
        }

        private static void ConfigureColorPickerButtonVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuColorPickerData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            var color = profile.GetData(data.Reference, data.Default);
            var colorElement = element.Q<VisualElement>("Color");
            colorElement.SetBackgroundColor(color);
        }

        private static void ConfigureColorPickerButtonInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuColorPickerData data)
        {
            var colorElement = element.Q<VisualElement>("Color");
            var button = element.Q<Button>("Button");
            button.clicked += () =>
            {
                menu.Populate(false, data.Name, null, () =>
                {
                    menu.AddElementToScrollView(CreateColorPicker(menu, data, 
                        callback: UpdateColorPickerButtonVisuals(menu.Profile, colorElement, data.Reference)));
                });
            };
        }

        private static Action<string, Color> UpdateColorPickerButtonVisuals(
            UIMenuDataProfile profile,
            VisualElement element,
            string reference)
        {
            return (string reference, Color color) =>
            {
                element.SetBackgroundColor(color);

                profile.OnColorPickerValueChanged(reference, color);
            };
        }
    }
}