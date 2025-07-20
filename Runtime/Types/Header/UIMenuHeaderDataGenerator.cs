using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuHeaderDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuHeaderData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuHeaderData typedData)
                    using (var generator = new UIMenuHeaderDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
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
            label.text = data.Name;
            label.style.marginTop = data.MarginTop;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuHeaderData data) { }

        public void Dispose() { }
    }
}