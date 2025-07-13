using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuToggleDataGenerator : UIMenuGeneratorTypeBase<UIMenuToggleData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory()
        {
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuToggleData toggleData)
                    return;

                using (var toggleDataGenerator = new UIMenuToggleDataGenerator())
                    generator.AddElementToScrollView(toggleDataGenerator.CreateElement(generator, toggleData));
            };
        }

        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuToggleData data)
        {
            const string ResourcePath = Path + "Toggle_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuToggleData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var value = menu.Profile.GetData(data.Reference, data.Default);

            var toggle = element.Q<Toggle>("Toggle");
            toggle.value = value;
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuToggleData data)
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