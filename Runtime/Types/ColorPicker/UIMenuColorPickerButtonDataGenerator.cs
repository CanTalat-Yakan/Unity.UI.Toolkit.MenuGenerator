using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuColorPickerButtonDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuColorPickerData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuColorPickerData typedData)
                    using (var generator = new UIMenuColorPickerButtonDataGenerator())
                        menu.AddElementToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "ColorPickerButton_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuColorPickerData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuColorPickerData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            var color = menu.Profile.GetData(data.Reference, data.Default);
            var colorElement = element.Q<VisualElement>("Color");
            colorElement.SetBackgroundColor(color);
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuColorPickerData data)
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