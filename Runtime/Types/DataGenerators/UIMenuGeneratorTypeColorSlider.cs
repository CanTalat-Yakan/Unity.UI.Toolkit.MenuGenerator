using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateColorSlider(UIMenuGenerator menu, UIMenuColorSliderData data)
        {
            var element = menu.Data.ColorSliderTemplate.CloneTree();
            ConfigureSliderVisuals(menu.Profile, element, data);
            ConfigureSliderInteraction(menu.Profile, element, data);
            return element;
        }

        private static void ConfigureSliderVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuColorSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var value = profile.GetData(data.Reference, data.Default);

            var icon = element.Q<VisualElement>("Icon");
            icon.SetBackgroundColor(data.Gradient.Evaluate(value / 100f));

            var slider = element.Q<SliderInt>();
            slider.lowValue = 0;
            slider.highValue = 100;
            slider.value = (int)value;
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