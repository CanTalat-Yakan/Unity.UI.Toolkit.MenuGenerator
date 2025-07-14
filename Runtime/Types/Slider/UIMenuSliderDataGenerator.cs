using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSliderDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuSliderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuSliderData typedData)
                    using (var generator = new UIMenuSliderDataGenerator())
                        menu.AddElementToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string SliderResourcePath = Path + "Slider_UXML";
        public static readonly string SliderIntResourcePath = Path + "SliderInt_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuSliderData data)
        {
            var resourcePath = data.IsFloat ? SliderResourcePath : SliderIntResourcePath;
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(resourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuSliderData data)
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

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuSliderData data)
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