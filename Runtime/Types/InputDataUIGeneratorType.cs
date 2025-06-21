using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "InputData_", menuName = "UI/Data/Input", order = 1)]
    public class InputData : ScriptableObject
    {
        public string Name;
        public string Reference;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateInput(UIMenuGenerator menu, InputData data)
        {
            var element = menu.Data.InputTemplate.CloneTree();

            ConfigureInputVisuals(menu.Profile, element, data);
            ConfigureInputInteraction(menu.Profile, element, data);

            return element;
        }

        private static void ConfigureInputVisuals(UIMenuDataProfile profile, VisualElement element, InputData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var inputField = element.Q<TextField>("Input");

            string input = string.Empty;
            profile.InputDataDictionary.TryGetValue(data.Reference, out input);

            inputField.value = input;
        }

        private static void ConfigureInputInteraction(UIMenuDataProfile profile, VisualElement element, InputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((evt) =>
            {
                profile.OnInputValueChanged(data.Reference, evt.newValue);
            });
        }
    }
}