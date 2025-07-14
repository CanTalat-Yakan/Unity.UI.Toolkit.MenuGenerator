using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuInputDataGenerator : UIMenuTypeDataGeneratorBase<UIMenuInputData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void RegisterFactory() =>
            UIMenuGenerator.RegisterTypeFactory += (menu, data) =>
            {
                if (data is UIMenuInputData typedData)
                    using (var generator = new UIMenuInputDataGenerator())
                        menu.AddToScrollView(generator.CreateElement(menu, typedData));
            };

        public static readonly string ResourcePath = Path + "Input_UXML";
        public override VisualElement CreateElement(UIMenuGenerator menu, UIMenuInputData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuGenerator menu, VisualElement element, UIMenuInputData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var inputField = element.Q<TextField>("Input");

            var input = menu.Profile.GetData(data.Reference, data.Default);

            if (string.IsNullOrEmpty(input))
                input = string.Empty;

            inputField.value = input;
        }

        public override void ConfigureInteraction(UIMenuGenerator menu, VisualElement element, UIMenuInputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((evt) =>
                menu.Profile.SetData(data.Reference, evt.newValue));
        }

        public void Dispose() { }
    }
}