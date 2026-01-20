using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class MenuSpacerDataGenerator : MenuTypeDataGeneratorBase<MenuSpacerData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            MenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is MenuSpacerData typedData)
                    using (var generator = new MenuSpacerDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Spacer_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, MenuSpacerData data)
        {
            var element = AssetResolver.TryGet<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, MenuSpacerData data)
        {
            var spacer = element.Q<VisualElement>("Spacer");
            spacer.SetHeight(data.Height);
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, MenuSpacerData data) { }

        public void Dispose() { }
    }
}