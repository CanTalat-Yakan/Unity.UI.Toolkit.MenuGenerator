using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "SliderData_", menuName = "UI/Data/Slider", order = 3)]
    public class SliderData : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public Vector2 ValueRange;
        public bool Float;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSlider(UIMenuGenerator menu, SliderData data)
        {
            VisualElement element = data.Float
                ? menu.UIGeneratorData.SliderTemplate.CloneTree()
                : menu.UIGeneratorData.SliderIntTemplate.CloneTree();

            ConfigureSliderVisuals(menu.Profile, element, data);
            ConfigureSliderInteraction(menu.Profile, element, data);

            return element;
        }

        private static void ConfigureSliderVisuals(UIMenuDataProfile profile, VisualElement element, SliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            float value = 0;
            profile.SliderDataDictionary.TryGetValue(data.Reference, out value);

            if (data.Float)
            {
                var slider = element.Q<Slider>("Slider");
                (slider.lowValue, slider.highValue) = (data.ValueRange.x, data.ValueRange.y);
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                (sliderInt.lowValue, sliderInt.highValue) = ((int)data.ValueRange.x, (int)data.ValueRange.y);
            }
        }

        private static void ConfigureSliderInteraction(UIMenuDataProfile profile, VisualElement element, SliderData data)
        {
            if (data.Float)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback(evt =>
                {
                    profile.OnSliderChange(data.Reference, evt.newValue);
                });
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback(evt =>
                {
                    profile.OnSliderChange(data.Reference, (float)evt.newValue);
                });
            }
        }
    }
}