using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuColorSliderData : UIGeneratorTypeTemplate
    {
        public string Name;
        public string Reference;

        [Space]
        public Gradient Gradient;
        [Range(0f, 1f)]
        public float Weight;

        public UIMenuColorSliderData SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            Name = name;
            Reference = name.ToLower().Replace(" ", "_");
            return this;
        }
    }

    public class UIMenuColorSliderDataGroup : UIGeneratorTypeTemplate
    {
        public string Name;
        public string Reference;

        [Space]
        public UIMenuColorSliderData[] ColorSliderData;

        public UIMenuColorSliderDataGroup SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            base.name = uniqueName;
            Name = name;
            Reference = name.ToLower().Replace(" ", "_");
            return this;
        }
    }

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

            var value = 0;
            profile.ColorSliderDataDictionary.TryGetValue(data.Reference, out value);

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