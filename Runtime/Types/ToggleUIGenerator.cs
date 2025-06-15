using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "ToggleData_", menuName = "UI/Data/Toggle", order = 4)]
    public class ToggleData : ScriptableObject
    {
        public string Name;
        public string Reference;
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddToggle(ToggleData data)
        {
            var element = CreateToggle(data);

            AddElementToScrollView(element);
        }

        private VisualElement CreateToggle(ToggleData data)
        {
            var element = UIGeneratorData.ToggleTemplate.CloneTree();

            ConfigureToggleVisuals(element, data);
            ConfigureToggleInteraction(element, data);

            return element;
        }

        private void ConfigureToggleVisuals(VisualElement element, ToggleData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var toggle = element.Q<Toggle>("Toggle");
            if (Profile.ToggleDataDictionary.TryGetValue(data.Reference, out bool value))
                toggle.value = value;
        }

        private void ConfigureToggleInteraction(VisualElement element, ToggleData data)
        {
            var toggle = element.Q<Toggle>("Toggle");
            toggle.RegisterValueChangedCallback(evt =>
            {
                Profile.OnToggleChange(data.Reference, evt.newValue);
            });
        }
    }
}