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

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddSlider(SliderData data)
        {
            var element = CreateSlider(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateSlider(SliderData data)
        {
            VisualElement element = data.Float
                ? UIGeneratorData.SliderTemplate.CloneTree()
                : UIGeneratorData.SliderIntTemplate.CloneTree();

            ConfigureSliderVisuals(element, data);
            ConfigureSliderInteraction(element, data);

            return element;
        }

        private void ConfigureSliderVisuals(VisualElement element, SliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            float value = 0;
            Profile.SliderDataDictionary.TryGetValue(data.Reference, out value);

            if (data.Float)
            {
                var slider = element.Q<Slider>("Slider");
                SetSliderRange(slider, (data.ValueRange.x, data.ValueRange.y), value);
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                SetSliderRange(sliderInt, ((int)data.ValueRange.x, (int)data.ValueRange.y), (int)value);
            }
        }

        private void ConfigureSliderInteraction(VisualElement element, SliderData data)
        {
            if (data.Float)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback(evt =>
                {
                    Profile.OnSliderChange(data.Reference, evt.newValue);
                });
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback(evt =>
                {
                    Profile.OnSliderChange(data.Reference, (float)evt.newValue);
                });
            }
        }
    }

    // Utility Methods
    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void SetSliderRange(Slider slider, (float min, float max) valueRange, float overrideValue = 0)
        {
            (slider.lowValue, slider.highValue) = valueRange;

            slider.value = overrideValue;
        }

        private void SetSliderRange(SliderInt sliderInt, (int min, int max) valueRange, int overrideValue = 0)
        {
            (sliderInt.lowValue, sliderInt.highValue) = valueRange;

            sliderInt.value = overrideValue;
        }
    }
}