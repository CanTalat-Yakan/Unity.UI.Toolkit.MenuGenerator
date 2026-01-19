using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuColorPickerDataGenerator : MenuTypeDataGeneratorBase<MenuColorPickerData>, IDisposable
    {
        public static readonly string ResourcePath = Path + "ColorPicker_UXML";

        public override VisualElement CreateElement(MenuGenerator menu, MenuColorPickerData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuColorPickerData data)
        {
            var hueSlider = element.Q<SliderInt>("HueSlider");
            var satSlider = element.Q<SliderInt>("SaturationSlider");
            var valSlider = element.Q<SliderInt>("ValueSlider");
            var alphaSlider = element.Q<SliderInt>("AlphaSlider");
            var colorElement = element.Q<VisualElement>("Color");

            element.Q<GroupBox>("Alpha").SetDisplayEnabled(data.HasAlpha);

            var color = menu.Profile2.Value.Get(data.Reference, data.Default);

            Color.RGBToHSV(color, out var h, out var s, out var v);
            hueSlider.value = (int)(h * 360);
            satSlider.value = (int)(s * 100);
            valSlider.value = (int)(v * 100);
            alphaSlider.value = (int)(color.a * 100);

            colorElement.SetBackgroundColor(color);
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuColorPickerData data)
        {
            var presetButtons = element.Query<Button>(name: "ColorPresetButton").ToList();
            var hueSlider = element.Q<SliderInt>("HueSlider");
            var satSlider = element.Q<SliderInt>("SaturationSlider");
            var valSlider = element.Q<SliderInt>("ValueSlider");
            var alphaSlider = element.Q<SliderInt>("AlphaSlider");
            var colorElement = element.Q<VisualElement>("Color");

            Action updateColor = () =>
            {
                var newColor = Color.HSVToRGB(
                    hueSlider.value / 360f,
                    satSlider.value / 100f,
                    valSlider.value / 100f);
                newColor.a = alphaSlider.value / 100f;

                colorElement.SetBackgroundColor(newColor);

                menu.Profile2.Value.Set(data.Reference, data.Default);
            };

            hueSlider.RegisterValueChangedCallback(_ => updateColor());
            satSlider.RegisterValueChangedCallback(_ => updateColor());
            valSlider.RegisterValueChangedCallback(_ => updateColor());
            alphaSlider.RegisterValueChangedCallback(_ => updateColor());

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

                    menu.Profile2.Value.Set(data.Reference, data.Default);
                };
        }

        public void Dispose()
        {
        }
    }
}