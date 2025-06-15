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

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddInput(InputData data)
        {
            var element = CreateInput(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateInput(InputData data)
        {
            var element = UIGeneratorData.InputTemplate.CloneTree();

            ConfigureInputVisuals(element, data);
            ConfigureInputInteraction(element, data);

            return element;
        }

        private void ConfigureInputVisuals(VisualElement element, InputData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var inputField = element.Q<TextField>("Input");

            string input = "";
            //Profile.InputDataDictionary.TryGetValue(data.Reference, out input);

            inputField.value = input;
        }

        private void ConfigureInputInteraction(VisualElement element, InputData data)
        {
            var textField = element.Q<TextField>("Input");
            textField.RegisterValueChangedCallback((e) =>
            {
                //Profile.OnInputChange(data.Reference, e.newValue);
            });
        }
    }
}