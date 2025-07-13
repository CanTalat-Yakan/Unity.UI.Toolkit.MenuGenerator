using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        private static VisualElement CreateColorPicker(
            UIMenuDataGenerator menu,
            UIMenuColorPickerData data,
            Action<string, Color> callback)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + "ColorPicker_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureColorSliders(menu.Profile, element, data, callback);
            ConfigureColorPresets(menu.Profile, element, data, callback);
            return element;
        }

        private static void ConfigureColorSliders(
            UIMenuDataProfile profile,
            VisualElement picker,
            UIMenuColorPickerData data,
            Action<string, Color> callback)
        {
            var hueSlider = picker.Q<SliderInt>("HueSlider");
            var satSlider = picker.Q<SliderInt>("SaturationSlider");
            var valSlider = picker.Q<SliderInt>("ValueSlider");
            var alphaSlider = picker.Q<SliderInt>("AlphaSlider");
            var colorElement = picker.Q<VisualElement>("Color");

            picker.Q<GroupBox>("Alpha").SetDisplayEnabled(data.HasAlpha);

            var color = profile.GetData(data.Reference, data.Default);

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

                callback.Invoke(data.Reference, newColor);

                colorElement.SetBackgroundColor(newColor);

                profile.OnColorPickerValueChanged(data.Reference, newColor);
            };

            hueSlider.RegisterValueChangedCallback(_ => updateColor());
            satSlider.RegisterValueChangedCallback(_ => updateColor());
            valSlider.RegisterValueChangedCallback(_ => updateColor());
            alphaSlider.RegisterValueChangedCallback(_ => updateColor());
        }

        private static void ConfigureColorPresets(
            UIMenuDataProfile profile,
            VisualElement picker,
            UIMenuColorPickerData data,
            Action<string, Color> callback)
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

                    callback.Invoke(data.Reference, updatedColor);

                    profile.OnColorPickerValueChanged(data.Reference, updatedColor);
                };
        }
    }
}