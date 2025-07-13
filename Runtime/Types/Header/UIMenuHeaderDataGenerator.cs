using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuHeaderDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuHeaderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory() =>
            UIMenuGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuHeaderData headerData)
                    return;

                using (var headerDataGenerator = new UIMenuHeaderDataGenerator())
                    generator.AddElementToScrollView(headerDataGenerator.CreateElement(generator, headerData));
            };

        public static readonly string ResourcePath = Path + "Header_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuHeaderData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuHeaderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();
            label.style.marginTop = data.MarginTop;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuHeaderData data) { }

        public void Dispose() { }
    }
}