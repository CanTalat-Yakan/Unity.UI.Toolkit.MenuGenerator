using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "ColorSliderData_", menuName = "UI/Data/Colors/Color Slider", order = 2)]
    public class ColorSliderData : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public Gradient Gradient;

        [Space]
        [Range(0f, 1f)]
        public float Weight;
    }

    [CreateAssetMenu(fileName = "ColorSliderDataGroup_", menuName = "UI/Data/Colors/Color Slider Group", order = 3)]
    public class ColorSliderDataGroup : ScriptableObject
    {
        public string Name;
        public string Reference;

        [Space]
        public ColorSliderData[] ColorSliderData;
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddColorSlider(ColorSliderData data)
        {
            var element = CreateColorSlider(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateColorSlider(ColorSliderData data)
        {
            var element = UIGeneratorData.ColorSliderTemplate.CloneTree();

            ConfigureSliderVisuals(element, data);
            ConfigureSliderInteraction(element, data);

            return element;
        }

        private void ConfigureSliderVisuals(VisualElement element, ColorSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var value = 0;
            Profile.ColorSliderDataDictionary.TryGetValue(data, out value);

            var icon = element.Q<VisualElement>("Icon");
            icon.SetBackgroundColor(data.Gradient.Evaluate(value / 100f));

            var slider = element.Q<SliderInt>();
            SetSliderRange(slider, (0, 100), value);
        }

        private void ConfigureSliderInteraction(VisualElement element, ColorSliderData data)
        {
            var icon = element.Q<VisualElement>("Icon");
            var slider = element.Q<SliderInt>();
            slider.RegisterValueChangedCallback(e =>
            {
                icon.SetBackgroundColor(data.Gradient.Evaluate(e.newValue / 100f));

                Profile.OnColorSliderChange(data, e.newValue);
            });
        }
    }
}