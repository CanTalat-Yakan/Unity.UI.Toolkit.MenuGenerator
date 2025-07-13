using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSlider(UIMenuDataGenerator menu, UIMenuSliderData data)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + (data.IsFloat ? "Slider_UXML" : "SliderInt_UXML");
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureSliderVisuals(menu.Profile, element, data);
            ConfigureSliderInteraction(menu.Profile, element, data);
            return element;
        }

        private static void ConfigureSliderVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var defaultValue = Mathf.Clamp(data.Default, data.MinRange, data.MaxRange);

            var value = profile.GetData(data.Reference, defaultValue);

            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.lowValue = data.MinRange;
                slider.highValue = data.MaxRange;
                slider.value = value;
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.lowValue = (int)data.MinRange;
                sliderInt.highValue = (int)data.MaxRange;
                sliderInt.value = (int)value;
            }
        }

        private static void ConfigureSliderInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuSliderData data)
        {
            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback((evt) =>
                    profile.OnSliderValueChanged(data.Reference, evt.newValue));
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback((evt) =>
                    profile.OnSliderValueChanged(data.Reference, evt.newValue));
            }
        }
    }
}