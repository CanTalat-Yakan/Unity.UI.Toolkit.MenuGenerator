using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuInputDataGenerator : UIMenuGeneratorTypeBase<UIMenuInputData>, IDisposable
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Factory()
        {
            UIMenuDataGenerator.RegisterTypeFactory += (generator, data) =>
            {
                if (data is not UIMenuInputData inputData)
                    return;

                using (var inputDataGenerator = new UIMenuInputDataGenerator())
                    generator.AddElementToScrollView(inputDataGenerator.CreateElement(generator, inputData));
            };
        }

        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuInputData data)
        {
            const string resourcePath = Path + "Input_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(resourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuInputData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var inputField = element.Q<TextField>("Input");

            var input = menu.Profile.GetData(data.Reference, data.Default);

            if (string.IsNullOrEmpty(input))
                input = string.Empty;

            inputField.value = input;
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuInputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((evt) =>
                menu.Profile.OnInputValueChanged(data.Reference, evt.newValue));
        }

        public void Dispose() { }
    }
}