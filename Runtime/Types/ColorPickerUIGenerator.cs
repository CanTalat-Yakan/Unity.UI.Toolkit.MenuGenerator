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

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddColorPickerButton(ColorPickerDataGroup group)
        {
            var element = CreateColorPickerButton(group);

            AddElementToScrollView(element);
        }

        private VisualElement CreateColorPickerButton(ColorPickerDataGroup group)
        {
            var element = UIGeneratorData.SelectionCategoryTemplate.CloneTree();

            ConfigureColorPickerButtonVisuals(element, group);
            ConfigureColorPickerButtonInteraction(element, group);

            return element;
        }

        private VisualElement CreateColorPickerButton(string name, string reference)
        {
            var group = new ColorPickerDataGroup()
            {
                Name = name,
                Reference = reference,
                ColorPickerData = new ColorPickerData[] { new() { Name = name, Reference = reference } }
            };

            return CreateColorPickerButton(group);
        }

        private void ConfigureColorPickerButtonVisuals(VisualElement element, ColorPickerDataGroup group)
        {
            var button = element.Q<Button>("Button");
            button.text = group.Name.ToUpper();

            var image = element.Q<VisualElement>("Image");

            if (Profile.ColorPickerDataDictionary.TryGetValue(group.Reference, out Color color))
                image.SetBackgroundColor(color);
        }

        private void ConfigureColorPickerButtonInteraction(VisualElement element, ColorPickerDataGroup group)
        {
            var button = element.Q<Button>();
            button.clicked += () => ShowColorPickerOverlay(group, UpdateColorPickerVisuals(element, group.Reference));
        }
    }

    // Overlay Management - Color Picker
    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void ShowColorPickerOverlay(ColorPickerDataGroup group, Action<string, Color> callback)
        {
            var overlay = CreatePopup(group.Name);

            foreach (var colorPickerData in group.ColorPickerData)
                overlay.Q<GroupBox>("GroupBox").Add(CreateColorPicker(colorPickerData, callback));

            Root.LinkedElement.Add(overlay);
        }

        private VisualElement CreateColorPicker(ColorPickerData data, Action<string, Color> callback)
        {
            var picker = UIGeneratorData.ColorPickerTemplate.CloneTree();

            ConfigureColorSliders(picker, data, callback);
            ConfigureColorPresets(picker, data, callback);

            return picker;
        }

        private void ConfigureColorSliders(VisualElement picker, ColorPickerData data, Action<string, Color> callback)
        {
            var hueSlider = picker.Q<SliderInt>("HueSlider");
            var satSlider = picker.Q<SliderInt>("SaturationSlider");
            var valSlider = picker.Q<SliderInt>("ValueSlider");
            var alphaSlider = picker.Q<SliderInt>("AlphaSlider");

            picker.Q<GroupBox>("Alpha").SetDisplayEnabled(data.HasAlpha);

            if (Profile.ColorPickerDataDictionary.TryGetValue(data.Reference, out Color color))
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

                Profile.OnColorPickerChange(data.Reference, newColor);
            };

            hueSlider.RegisterValueChangedCallback(_ => updateColor());
            satSlider.RegisterValueChangedCallback(_ => updateColor());
            valSlider.RegisterValueChangedCallback(_ => updateColor());
            alphaSlider.RegisterValueChangedCallback(_ => updateColor());
        }

        private void ConfigureColorPresets(VisualElement picker, ColorPickerData data, Action<string, Color> callback)
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

                    Profile.OnColorPickerChange(data.Reference, updatedColor);
                };
            }
        }

        private Action<string, Color> UpdateColorPickerVisuals(VisualElement element, string reference) =>
            (string reference, Color color) =>
            {
                element.Q<VisualElement>("Image").SetBackgroundColor(color);

                Profile.OnColorPickerChange(reference, color);
            };
    }
}