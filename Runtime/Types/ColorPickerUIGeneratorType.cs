using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "ColorPickerData_", menuName = "UI/Data/Colors/Color Picker", order = 0)]
    public class ColorPickerData : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public bool HasAlpha;
    }

    [CreateAssetMenu(fileName = "ColorPickerDataGroup_", menuName = "UI/Data/Colors/Color Picker Group", order = 1)]
    public class ColorPickerDataGroup : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public ColorPickerData[] ColorPickerData;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateColorPickerButton(UIMenuGenerator menu, ColorPickerDataGroup group)
        {
            var element = menu.Data.SelectionCategoryTemplate.CloneTree();

            ConfigureColorPickerButtonVisuals(menu.Profile, element, group);
            ConfigureColorPickerButtonInteraction(menu, element, group);

            return element;
        }

        private static VisualElement CreateColorPickerButton(UIMenuGenerator menu, string name, string reference)
        {
            var group = new ColorPickerDataGroup()
            {
                Name = name,
                Reference = reference,
                ColorPickerData = new ColorPickerData[] { new() { Name = name, Reference = reference } }
            };

            return CreateColorPickerButton(menu, group);
        }

        private static void ConfigureColorPickerButtonVisuals(UIMenuDataProfile profile, VisualElement element, ColorPickerDataGroup group)
        {
            var button = element.Q<Button>("Button");
            button.text = group.Name.ToUpper();

            var image = element.Q<VisualElement>("Image");

            if (profile.ColorPickerDataDictionary.TryGetValue(group.Reference, out Color color))
                image.SetBackgroundColor(color);
        }

        private static void ConfigureColorPickerButtonInteraction(UIMenuGenerator menu, VisualElement element, ColorPickerDataGroup group)
        {
            var button = element.Q<Button>();
            button.clicked += () =>
                ShowColorPickerOverlay(menu, group, callback:
                    UpdateColorPickerVisuals(menu.Profile, element, group.Reference));
        }
    }

    // Overlay Management - Color Picker
    public static partial class UIMenuGeneratorType
    {
        private static void ShowColorPickerOverlay(UIMenuGenerator menu, ColorPickerDataGroup group, Action<string, Color> callback)
        {
            var overlay = menu.CreatePopup(group.Name);

            foreach (var colorPickerData in group.ColorPickerData)
                overlay.Q<GroupBox>("GroupBox").Add(CreateColorPicker(menu, colorPickerData, callback));

            menu.AddElementToRoot(overlay);
        }

        private static VisualElement CreateColorPicker(UIMenuGenerator menu, ColorPickerData data, Action<string, Color> callback)
        {
            var picker = menu.Data.ColorPickerTemplate.CloneTree();

            ConfigureColorSliders(menu.Profile, picker, data, callback);
            ConfigureColorPresets(menu.Profile, picker, data, callback);

            return picker;
        }

        private static void ConfigureColorSliders(UIMenuDataProfile profile, VisualElement picker, ColorPickerData data, Action<string, Color> callback)
        {
            var hueSlider = picker.Q<SliderInt>("HueSlider");
            var satSlider = picker.Q<SliderInt>("SaturationSlider");
            var valSlider = picker.Q<SliderInt>("ValueSlider");
            var alphaSlider = picker.Q<SliderInt>("AlphaSlider");

            picker.Q<GroupBox>("Alpha").SetDisplayEnabled(data.HasAlpha);

            if (profile.ColorPickerDataDictionary.TryGetValue(data.Reference, out Color color))
            {
                Color.RGBToHSV(color, out float h, out float s, out float v);
                hueSlider.value = (int)(h * 360);
                satSlider.value = (int)(s * 100);
                valSlider.value = (int)(v * 100);
                alphaSlider.value = (int)(color.a * 100);
            }

            Action updateColor = () =>
            {
                var newColor = Color.HSVToRGB(
                    hueSlider.value / 360f,
                    satSlider.value / 100f,
                    valSlider.value / 100f);
                newColor.a = alphaSlider.value / 100f;

                callback(data.Reference, newColor);

                profile.OnColorPickerValueChanged(data.Reference, newColor);
            };

            hueSlider.RegisterValueChangedCallback(_ => updateColor());
            satSlider.RegisterValueChangedCallback(_ => updateColor());
            valSlider.RegisterValueChangedCallback(_ => updateColor());
            alphaSlider.RegisterValueChangedCallback(_ => updateColor());
        }

        private static void ConfigureColorPresets(UIMenuDataProfile profile, VisualElement picker, ColorPickerData data, Action<string, Color> callback)
        {
            var presetButtons = picker.Query<Button>(name: "ColorPresetButton").ToList();
            var hueSlider = picker.Q<SliderInt>("HueSlider");
            var satSlider = picker.Q<SliderInt>("SaturationSlider");
            var valSlider = picker.Q<SliderInt>("ValueSlider");
            var alphaSlider = picker.Q<SliderInt>("AlphaSlider");

            foreach (var button in presetButtons)
            {
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
        }

        private static Action<string, Color> UpdateColorPickerVisuals(UIMenuDataProfile profile, VisualElement element, string reference) =>
            (string reference, Color color) =>
            {
                element.Q<VisualElement>("Image").SetBackgroundColor(color);

                profile.OnColorPickerValueChanged(reference, color);
            };
    }
}