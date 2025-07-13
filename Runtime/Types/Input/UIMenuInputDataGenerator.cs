using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateInput(UIMenuDataGenerator menu, UIMenuInputData data)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + "Input_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureInputVisuals(menu.Profile, element, data);
            ConfigureInputInteraction(menu.Profile, element, data);
            return element;
        }

        private static void ConfigureInputVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuInputData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var inputField = element.Q<TextField>("Input");

            var input = profile.GetData(data.Reference, data.Default);

            if (string.IsNullOrEmpty(input))
                input = string.Empty;

            inputField.value = input;
        }

        private static void ConfigureInputInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuInputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((evt) =>
                profile.OnInputValueChanged(data.Reference, evt.newValue));
        }
    }
}