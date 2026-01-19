using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuSliderDataGenerator : MenuTypeDataGeneratorBase<MenuSliderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuSliderData typedData)
                    using (var generator = new MenuSliderDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string SliderResourcePath = Path + "Slider_UXML";
        public static readonly string SliderIntResourcePath = Path + "SliderInt_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuSliderData data)
        {
            var resourcePath = data.IsFloat ? SliderResourcePath : SliderIntResourcePath;
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(resourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;

            var defaultValue = Mathf.Clamp(data.Default, data.MinValue, data.MaxValue);
            defaultValue = data.IsFloat ? defaultValue : (int)defaultValue;

            var value = menu.Profile2.Value.Get(data.Reference, defaultValue);

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

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuSliderData data)
        {
            if (data.IsFloat)
            {
                var slider = element.Q<Slider>("Slider");
                slider.RegisterValueChangedCallback((evt) =>
                    menu.Profile2.Value.Set(data.Reference, evt.newValue));
            }
            else
            {
                var sliderInt = element.Q<SliderInt>("Slider");
                sliderInt.RegisterValueChangedCallback((evt) =>
                    menu.Profile2.Value.Set(data.Reference, (float)evt.newValue));
            }
        }

        public void Dispose() { }
    }
}