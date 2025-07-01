using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuColorSliderData : UIGeneratorTypeTemplate
    {
        [Space]
        public Gradient Gradient;
        [Range(0f, 1f)]
        public float Default;
    }

    public class UIMenuColorSliderDataGroup : UIGeneratorTypeTemplate
    {
        [Space]
        public UIMenuColorSliderData[] ColorSliderData;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateColorSlider(UIMenuGenerator menu, UIMenuColorSliderData data)
        {
            menu.Profile.ColorSliderDataDictionary.TryAdd(data.Reference, data.Default);

            var element = menu.Data.ColorSliderTemplate.CloneTree();
            ConfigureSliderVisuals(menu.Profile, element, data);
            ConfigureSliderInteraction(menu.Profile, element, data);
            return element;
        }

        private static void ConfigureSliderVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuColorSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            if (!profile.ColorSliderDataDictionary.TryGetValue(data.Reference, out var value))
                value = data.Default;

            var icon = element.Q<VisualElement>("Icon");
            icon.SetBackgroundColor(data.Gradient.Evaluate(value / 100f));

            var slider = element.Q<SliderInt>(); 
            (slider.lowValue, slider.highValue) = (0, 100);
        }

        private static void ConfigureSliderInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuColorSliderData data)
        {
            var icon = element.Q<VisualElement>("Icon");
            var slider = element.Q<SliderInt>();
            slider.RegisterValueChangedCallback(e =>
            {
                icon.SetBackgroundColor(data.Gradient.Evaluate(e.newValue / 100f));

                profile.OnColorSliderValueChanged(data.Reference, e.newValue);
            });
        }
    }
}