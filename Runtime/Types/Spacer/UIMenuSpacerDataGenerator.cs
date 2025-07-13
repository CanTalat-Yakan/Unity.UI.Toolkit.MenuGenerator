using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSpacerDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuSpacerData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory() =>
            UIMenuGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuSpacerData spacerData)
                    return;

                using (var spacerDataGenerator = new UIMenuSpacerDataGenerator())
                    generator.AddElementToScrollView(spacerDataGenerator.CreateElement(generator, spacerData));
            };

        public static readonly string ResourcePath = Path + "Spacer_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuSpacerData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuSpacerData data)
        {
            var spacer = element.Q<VisualElement>("Spacer");
            spacer.SetHeight(data.Height);
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuSpacerData data) { }

        public void Dispose() { }
    }
}