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

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateToggle(UIMenuGenerator menu, ToggleData data)
        {
            var element = menu.UIGeneratorData.ToggleTemplate.CloneTree();

            ConfigureToggleVisuals(menu.Profile, element, data);
            ConfigureToggleInteraction(menu.Profile, element, data);

            return element;
        }

        private static void ConfigureToggleVisuals(UIMenuDataProfile profile, VisualElement element, ToggleData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var toggle = element.Q<Toggle>("Toggle");
            if (profile.ToggleDataDictionary.TryGetValue(data.Reference, out bool value))
                toggle.value = value;
        }

        private static void ConfigureToggleInteraction(UIMenuDataProfile profile, VisualElement element, ToggleData data)
        {
            var toggle = element.Q<Toggle>("Toggle");
            toggle.RegisterValueChangedCallback(evt =>
            {
                profile.OnToggleChange(data.Reference, evt.newValue);
            });
        }
    }
}