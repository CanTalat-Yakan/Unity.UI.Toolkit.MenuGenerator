using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSpacerDataGenerator : UIMenuGeneratorTypeBase<UIMenuSpacerData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory()
        {
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuSpacerData spacerData)
                    return;

                using (var spacerDataGenerator = new UIMenuSpacerDataGenerator())
                    generator.AddElementToScrollView(spacerDataGenerator.CreateElement(generator, spacerData));
            };
        }

        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuSpacerData data)
        {
            const string ResourcePath = Path + "Spacer_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuSpacerData data)
        {
            var spacer = element.Q<VisualElement>("Spacer");
            spacer.SetHeight(data.Height);
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuSpacerData data) { }

        public void Dispose() { }
    }
}