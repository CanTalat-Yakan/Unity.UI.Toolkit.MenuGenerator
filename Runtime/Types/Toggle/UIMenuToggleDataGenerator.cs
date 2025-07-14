using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuToggleDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuToggleData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuToggleData typedData)
                    using (var generator = new UIMenuToggleDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Toggle_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuToggleData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuToggleData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var value = menu.Profile.GetData(data.Reference, data.Default);

            var toggle = element.Q<Toggle>("Toggle");
            toggle.value = value;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuToggleData data)
        {
            var toggle = element.Q<Toggle>("Toggle");
            toggle.RegisterValueChangedCallback((evt) =>
            {
                menu.Profile.SetData(data.Reference, evt.newValue);
            });
        }

        public void Dispose() { }
    }
}