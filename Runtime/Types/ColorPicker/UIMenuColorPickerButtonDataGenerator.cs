using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuColorPickerButtonDataGenerator : UIMenuGeneratorTypeBase<UIMenuColorPickerData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory()
        {
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuColorPickerData colorPickerData)
                    return;

                using (var colorPickerDataGenerator = new UIMenuColorPickerButtonDataGenerator())
                    generator.AddElementToScrollView(colorPickerDataGenerator.CreateElement(generator, colorPickerData));
            };
        }

        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuColorPickerData data)
        {
            var resourcePath = Path + "ColorPickerButton_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(resourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuColorPickerData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            var color = menu.Profile.GetData(data.Reference, data.Default);
            var colorElement = element.Q<VisualElement>("Color");
            colorElement.SetBackgroundColor(color);
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuColorPickerData data)
        {
            var colorElement = element.Q<VisualElement>("Color");
            var button = element.Q<Button>("Button");
            button.clicked += () =>
            {
                menu.Populate(false, data.Name, null, () =>
                {
                    using (var colorPickerGenerator = new UIMenuColorPickerDataGenerator())
                        menu.AddElementToScrollView(colorPickerGenerator.CreateElement(menu, data));
                });
            };
        }

        public void Dispose() { }
    }
}