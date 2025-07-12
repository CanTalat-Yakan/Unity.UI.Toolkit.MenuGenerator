using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateColorSlider(UIMenuDataGenerator menu, UIMenuColorSliderData data)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + "ColorSlider_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
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
            slider.RegisterValueChangedCallback((evt) =>
            {
                icon.SetBackgroundColor(data.Gradient.Evaluate(evt.newValue / 100f));

                profile.OnColorSliderValueChanged(data.Reference, evt.newValue);
            });
        }
    }
}