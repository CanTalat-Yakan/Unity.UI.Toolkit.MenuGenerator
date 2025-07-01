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
        public float Default;

        public Vector2 ValueRange => new Vector2(MinRange, MaxRange);
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSlider(UIMenuGenerator menu, UIMenuSliderData data)
        {
            menu.Profile.SliderDataDictionary.TryAdd(data.Reference, data.Default);

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

            if(!profile.SliderDataDictionary.TryGetValue(data.Reference, out var value))
                value = data.Default;

            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                (slider.lowValue, slider.highValue) = (data.ValueRange.x, data.ValueRange.y);
                slider.value = value;
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                (sliderInt.lowValue, sliderInt.highValue) = ((int)data.ValueRange.x, (int)data.ValueRange.y);
                sliderInt.value = (int)value;
            }
        }

        private static void ConfigureSliderInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuSliderData data)
        {
            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback(evt =>
                {
                    profile.OnSliderValueChanged(data.Reference, evt.newValue);
                });
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback(evt =>
                {
                    profile.OnSliderValueChanged(data.Reference, (float)evt.newValue);
                });
            }
        }
    }
}