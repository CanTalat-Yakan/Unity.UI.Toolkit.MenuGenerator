using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuHeaderDataGenerator : MenuTypeDataGeneratorBase<MenuHeaderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuHeaderData typedData)
                    using (var generator = new MenuHeaderDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Header_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuHeaderData data)
        {
            var element = ResourceLoader.TryGet<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuHeaderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name;
            label.style.marginTop = data.MarginTop;
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuHeaderData data) { }

        public void Dispose() { }
    }
}