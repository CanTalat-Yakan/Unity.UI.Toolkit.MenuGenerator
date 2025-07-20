using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuColorSliderDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuColorSliderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuColorSliderData typedData)
                    using (var generator = new UIMenuColorSliderDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "ColorSlider_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuColorSliderData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuColorSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;

            var value = menu.Profile.Get<int>(data);

            var icon = element.Q<VisualElement>("Icon");
            icon.SetBackgroundColor(data.Gradient.Evaluate(value / 100f));

            var sliderInt = element.Q<SliderInt>();
            sliderInt.lowValue = 0;
            sliderInt.highValue = 100;
            sliderInt.value = value;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuColorSliderData data)
        {
            var icon = element.Q<VisualElement>("Icon");
            var sliderInt = element.Q<SliderInt>();
            sliderInt.RegisterValueChangedCallback((EventCallback<ChangeEvent<int>>)((evt) =>
            {
                icon.SetBackgroundColor(data.Gradient.Evaluate(evt.newValue / 100f));

                menu.Profile.Set(data.Reference, evt.newValue);
            }));
        }

        public void Dispose() { }
    }
}