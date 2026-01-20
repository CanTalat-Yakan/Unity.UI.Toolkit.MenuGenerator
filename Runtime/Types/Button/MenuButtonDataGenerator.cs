using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuButtonDataGenerator : MenuTypeDataGeneratorBase<MenuButtonData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuButtonData typedData)
                    using (var generator = new MenuButtonDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Button_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuButtonData data)
        {
            var element = ResourceLoader.TryGet<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Name;

            if (data.Texture != null)
                element.Q<VisualElement>("Icon").SetBackgroundImage(data.Texture);
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuButtonData data)
        {
            var button = element.Q<Button>("Button");
            button.clicked += () => data.InvokeEvent();
        }

        public void Dispose() { }
    }
}