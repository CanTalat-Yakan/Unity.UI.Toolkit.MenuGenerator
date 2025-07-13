using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuButtonDataGenerator : UIMenuGeneratorTypeBase<UIMenuButtonData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory() =>
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuButtonData buttonData)
                    return;

                using (var buttonDataGenerator = new UIMenuButtonDataGenerator())
                    generator.AddElementToScrollView(buttonDataGenerator.CreateElement(generator, buttonData));
            };

        public static readonly string ResourcePath = Path + "Button_UXML";
        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuButtonData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name.ToUpper();

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => data.InvokeEvent();
        }

        public void Dispose() { }
    }
}