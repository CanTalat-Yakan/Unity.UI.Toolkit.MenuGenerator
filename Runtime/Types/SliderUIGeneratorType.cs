using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSliderData : UIGeneratorTypeTemplate
    {
        [Space]
        public bool IsFloat;
        public float MinRange = 0;
        public float MaxRange = 100;

        [Space]
        [Range(0, 1)]
        public float Default;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSlider(UIMenuGenerator menu, UIMenuSliderData data)
        {
            var element = data.IsFloat
                ? menu.Data.SliderTemplate.CloneTree()
                : menu.Data.SliderIntTemplate.CloneTree();
            ConfigureSliderVisuals(menu.Profile, element, data);
            ConfigureSliderInteraction(menu.Profile, element, data);
            return element;
        }

        private static void ConfigureSliderVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            profile.Sliders.TryGetValue(data.Reference, data.Default, out var value);

            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.lowValue = data.MinRange;
                slider.highValue = data.MaxRange;
                slider.value = value * data.MaxRange;
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.lowValue = (int)data.MinRange;
                sliderInt.highValue = (int)data.MaxRange;
                sliderInt.value = (int)(value * data.MaxRange);
            }
        }

        private static void ConfigureSliderInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuSliderData data)
        {
            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback(e =>
                    profile.OnSliderValueChanged(data.Reference, e.newValue));
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback(e =>
                    profile.OnSliderValueChanged(data.Reference, e.newValue));
            }
        }
    }
}