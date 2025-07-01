using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuInputData : UIGeneratorTypeTemplate
    {
        [Space]
        public string Default;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateInput(UIMenuGenerator menu, UIMenuInputData data)
        {
            var element = menu.Data.InputTemplate.CloneTree();

            ConfigureInputVisuals(menu.Profile, element, data);
            ConfigureInputInteraction(menu.Profile, element, data);

            return element;
        }

        private static void ConfigureInputVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuInputData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var inputField = element.Q<TextField>("Input");

            if(!profile.InputDataDictionary.TryGetValue(data.Reference, out var input))
                input = data.Default;

            if (string.IsNullOrEmpty(input))
                input = string.Empty;

            inputField.value = input;
        }

        private static void ConfigureInputInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuInputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((evt) =>
            {
                profile.OnInputValueChanged(data.Reference, evt.newValue);
            });
        }
    }
}