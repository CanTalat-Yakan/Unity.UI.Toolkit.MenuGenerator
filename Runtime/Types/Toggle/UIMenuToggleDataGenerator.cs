using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuToggleDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuToggleData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory() =>
            UIMenuGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuToggleData toggleData)
                    return;

                using (var toggleDataGenerator = new UIMenuToggleDataGenerator())
                    generator.AddElementToScrollView(toggleDataGenerator.CreateElement(generator, toggleData));
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
                menu.Profile.OnToggleValueChanged(data.Reference, evt.newValue);
            });
        }

        public void Dispose() { }
    }
}