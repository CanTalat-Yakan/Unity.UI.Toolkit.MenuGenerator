using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuColorSliderDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuColorSliderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory() =>
            UIMenuGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuColorSliderData colorSliderData)
                    return;

                using (var colorSliderDataGenerator = new UIMenuColorSliderDataGenerator())
                    generator.AddElementToScrollView(colorSliderDataGenerator.CreateElement(generator, colorSliderData));
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
            label.text = data.Name.ToUpper();

            var value = menu.Profile.GetData(data.Reference, data.Default);

            var icon = element.Q<VisualElement>("Icon");
            icon.SetBackgroundColor(data.Gradient.Evaluate(value / 100f));

            var slider = element.Q<SliderInt>();
            slider.lowValue = 0;
            slider.highValue = 100;
            slider.value = (int)value;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuColorSliderData data)
        {
            var icon = element.Q<VisualElement>("Icon");
            var slider = element.Q<SliderInt>();
            slider.RegisterValueChangedCallback((evt) =>
            {
                icon.SetBackgroundColor(data.Gradient.Evaluate(evt.newValue / 100f));

                menu.Profile.OnColorSliderValueChanged(data.Reference, evt.newValue);
            });
        }

        public void Dispose() { }
    }
}