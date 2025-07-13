using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSliderDataGenerator : UIMenuGeneratorTypeBase<UIMenuSliderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory()
        {
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuSliderData sliderData)
                    return;

                using (var sliderDataGenerator = new UIMenuSliderDataGenerator())
                    generator.AddElementToScrollView(sliderDataGenerator.CreateElement(generator, sliderData));
            };
        }

        public const string SliderResourcePath = Path + "Slider_UXML";
        public const string SliderIntResourcePath = Path + "SliderInt_UXML";
        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuSliderData data)
        {
            var resourcePath = data.IsFloat ? SliderResourcePath : SliderIntResourcePath;
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(resourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var defaultValue = Mathf.Clamp(data.Default, data.MinRange, data.MaxRange);
            defaultValue = data.IsFloat ? defaultValue : (int)defaultValue;

            var value = menu.Profile.GetData(data.Reference, defaultValue);

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

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuSliderData data)
        {
            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback((evt) =>
                    menu.Profile.OnSliderValueChanged(data.Reference, evt.newValue));
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback((evt) =>
                    menu.Profile.OnSliderValueChanged(data.Reference, evt.newValue));
            }
        }

        public void Dispose() { }
    }
}