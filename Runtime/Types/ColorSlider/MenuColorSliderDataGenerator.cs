using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuColorSliderDataGenerator : MenuTypeDataGeneratorBase<MenuColorSliderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuColorSliderData typedData)
                    using (var generator = new MenuColorSliderDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "ColorSlider_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuColorSliderData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuColorSliderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;

            var value = menu.Profile.Value.Get(data.Reference, data.Default);

            var icon = element.Q<VisualElement>("Icon");
            icon.SetBackgroundColor(data.Gradient.Evaluate(value / 100f));

            var sliderInt = element.Q<SliderInt>();
            sliderInt.lowValue = 0;
            sliderInt.highValue = 100;
            sliderInt.value = Mathf.CeilToInt(value);
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuColorSliderData data)
        {
            var icon = element.Q<VisualElement>("Icon");
            var sliderInt = element.Q<SliderInt>();
            sliderInt.RegisterValueChangedCallback((EventCallback<ChangeEvent<int>>)((evt) =>
            {
                icon.SetBackgroundColor(data.Gradient.Evaluate(evt.newValue / 100f));

                menu.Profile.Value.Set(data.Reference, evt.newValue);
            }));
        }

        public void Dispose() { }
    }
}