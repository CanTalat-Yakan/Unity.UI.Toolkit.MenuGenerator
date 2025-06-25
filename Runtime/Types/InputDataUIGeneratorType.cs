using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuInputData : ScriptableObject
    {
        public string Name;
        public string Reference;

        public UIMenuInputData SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            base.name = uniqueName;
            Name = name;
            Reference = name.ToLower().Replace(" ", "_");
            return this;
        }
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

            string input = string.Empty;
            profile.InputDataDictionary.TryGetValue(data.Reference, out input);

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