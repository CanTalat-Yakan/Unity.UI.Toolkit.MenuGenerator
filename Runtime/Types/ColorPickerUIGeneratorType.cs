using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuColorPickerData : UIGeneratorTypeTemplate
    {
        [Space]
        public bool HasAlpha;

        [Space]
        public Color Default;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateColorPickerButton(UIMenuGenerator menu, UIMenuColorPickerData data)
        {
            var element = menu.Data.SelectionCategoryTemplate.CloneTree();

            ConfigureColorPickerButtonVisuals(menu.Profile, element, data);
            ConfigureColorPickerButtonInteraction(menu, element, data);

            return element;
        }

        private static VisualElement CreateColorPickerButton(UIMenuGenerator menu, string name, string reference)
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

            var image = element.Q<VisualElement>("Image");

            if (!profile.ColorPickerDataDictionary.TryGetValue(data.Reference, out var color))
                color = data.Default;

            image.SetBackgroundColor(color);
        }

        private static void ConfigureColorPickerButtonInteraction(UIMenuGenerator menu, VisualElement element, UIMenuColorPickerData data)
        {
            var button = element.Q<Button>();
            button.clicked += () =>
                ShowColorPickerOverlay(menu, data, callback:
                    UpdateColorPickerVisuals(menu.Profile, element, data.Reference));
        }
    }

    // Overlay Management - Color Picker
    public static partial class UIMenuGeneratorType
    {
        private static void ShowColorPickerOverlay(UIMenuGenerator menu, UIMenuColorPickerData data, Action<string, Color> callback)
        {
            menu.PopulateHierarchy(false, data.Name, null);
            menu.AddElementToScrollView(CreateColorPicker(menu, data, callback));
        }

        private static VisualElement CreateColorPicker(UIMenuGenerator menu, UIMenuColorPickerData data, Action<string, Color> callback)
        {
            var picker = menu.Data.ColorPickerTemplate.CloneTree();

            ConfigureColorSliders(menu.Profile, picker, data, callback);
            ConfigureColorPresets(menu.Profile, picker, data, callback);

            return picker;
        }

        private static void ConfigureColorSliders(UIMenuDataProfile profile, VisualElement picker, UIMenuColorPickerData data, Action<string, Color> callback)
        {
            var hueSlider = picker.Q<SliderInt>("HueSlider");
            var satSlider = picker.Q<SliderInt>("SaturationSlider");
            var valSlider = picker.Q<SliderInt>("ValueSlider");
            var alphaSlider = picker.Q<SliderInt>("AlphaSlider");
            var colorElement = picker.Q<VisualElement>("Color");

            picker.Q<GroupBox>("Alpha").SetDisplayEnabled(data.HasAlpha);

            if (!profile.ColorPickerDataDictionary.TryGetValue(data.Reference, out var color))
                color = data.Default;

            Color.RGBToHSV(color, out var h, out var s, out var v);
            hueSlider.value = (int)(h * 360);
            satSlider.value = (int)(s * 100);
            valSlider.value = (int)(v * 100);
            alphaSlider.value = (int)(color.a * 100);
            colorElement.SetBackgroundColor(color);

            Action updateColor = () =>
            {
                var newColor = Color.HSVToRGB(
                    hueSlider.value / 360f,
                    satSlider.value / 100f,
                    valSlider.value / 100f);
                newColor.a = alphaSlider.value / 100f;

                colorElement.SetBackgroundColor(newColor);

                callback(data.Reference, newColor);

                profile.OnColorPickerValueChanged(data.Reference, newColor);
            };

            hueSlider.RegisterValueChangedCallback(_ => updateColor());
            satSlider.RegisterValueChangedCallback(_ => updateColor());
            valSlider.RegisterValueChangedCallback(_ => updateColor());
            alphaSlider.RegisterValueChangedCallback(_ => updateColor());
        }

        private static void ConfigureColorPresets(UIMenuDataProfile profile, VisualElement picker, UIMenuColorPickerData data, Action<string, Color> callback)
        {
            var presetButtons = picker.Query<Button>(name: "ColorPresetButton").ToList();
            var hueSlider = picker.Q<SliderInt>("HueSlider");
            var satSlider = picker.Q<SliderInt>("SaturationSlider");
            var valSlider = picker.Q<SliderInt>("ValueSlider");
            var alphaSlider = picker.Q<SliderInt>("AlphaSlider");

            foreach (var button in presetButtons)
                button.clicked += () =>
                {
                    var color = button.GetBackgroundColor();

                    Color.RGBToHSV(color, out float h, out float s, out float v);
                    hueSlider.value = (int)(h * 360);
                    satSlider.value = (int)(s * 100);
                    valSlider.value = (int)(v * 100);
                    alphaSlider.value = (int)(color.a * 100);

                    var updatedColor = Color.HSVToRGB(
                        hueSlider.value / 360f,
                        satSlider.value / 100f,
                        valSlider.value / 100f);
                    updatedColor.a = alphaSlider.value / 100f;

                    callback(data.Reference, updatedColor);

                    profile.OnColorPickerValueChanged(data.Reference, updatedColor);
                };
        }

        private static Action<string, Color> UpdateColorPickerVisuals(UIMenuDataProfile profile, VisualElement element, string reference) =>
            (string reference, Color color) =>
            {
                element.Q<VisualElement>("Image").SetBackgroundColor(color);

                profile.OnColorPickerValueChanged(reference, color);
            };
    }
}