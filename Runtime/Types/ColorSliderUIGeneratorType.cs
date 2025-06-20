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

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateColorSlider(UIMenuGenerator menu, ColorSliderData data)
        {
            var element = menu.Data.ColorSliderTemplate.CloneTree();

            ConfigureSliderVisuals(menu.Profile, element, data);
            ConfigureSliderInteraction(menu.Profile, element, data);

            return element;
        }

        private static void ConfigureSliderVisuals(UIMenuDataProfile profile, VisualElement element, ColorSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var value = 0;
            profile.ColorSliderDataDictionary.TryGetValue(data, out value);

            var icon = element.Q<VisualElement>("Icon");
            icon.SetBackgroundColor(data.Gradient.Evaluate(value / 100f));

            var slider = element.Q<SliderInt>(); 
            (slider.lowValue, slider.highValue) = (0, 100);
        }

        private static void ConfigureSliderInteraction(UIMenuDataProfile profile, VisualElement element, ColorSliderData data)
        {
            var icon = element.Q<VisualElement>("Icon");
            var slider = element.Q<SliderInt>();
            slider.RegisterValueChangedCallback(e =>
            {
                icon.SetBackgroundColor(data.Gradient.Evaluate(e.newValue / 100f));

                profile.OnColorSliderChange(data, e.newValue);
            });
        }
    }
}