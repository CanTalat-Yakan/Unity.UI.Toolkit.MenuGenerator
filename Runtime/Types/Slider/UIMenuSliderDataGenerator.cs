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
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
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
            label.text = data.Name;

            var defaultValue = Mathf.Clamp(data.Default, data.MinValue, data.MaxValue);
            defaultValue = data.IsFloat ? defaultValue : (int)defaultValue;

            var value = menu.Profile.Get(data.Reference, defaultValue);

            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.lowValue = data.MinValue;
                slider.highValue = data.MaxValue;
                slider.value = value;
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.lowValue = (int)data.MinValue;
                sliderInt.highValue = (int)data.MaxValue;
                sliderInt.value = (int)value;
            }
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuSliderData data)
        {
            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback((evt) =>
                    menu.Profile.Set(data.Reference, evt.newValue));
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback((evt) =>
                    menu.Profile.Set(data.Reference, (float)evt.newValue));
            }
        }

        public void Dispose() { }
    }
}