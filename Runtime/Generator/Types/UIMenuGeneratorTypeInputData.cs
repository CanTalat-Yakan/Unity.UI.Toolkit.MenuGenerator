using UnityEngine.UIElements;

namespace UnityEssentials
{
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

            profile.Inputs.TryGetValue(data.Reference, data.Default, out var input);

            if (string.IsNullOrEmpty(input))
                input = string.Empty;

            inputField.value = input;
        }

        private static void ConfigureInputInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuInputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((e) =>
                profile.OnInputValueChanged(data.Reference, e.newValue));
        }
    }
}