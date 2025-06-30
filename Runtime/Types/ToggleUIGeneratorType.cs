using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuToggleData : UIGeneratorTypeTemplate
    {
        public string Name;
        public string Reference;

        [Space]
        public bool Value;

        public UIMenuToggleData SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            Name = name;
            Reference = name.ToLower().Replace(" ", "_");
            return this;
        }
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateToggle(UIMenuGenerator menu, UIMenuToggleData data)
        {
            var element = menu.Data.ToggleTemplate.CloneTree();

            ConfigureToggleVisuals(menu.Profile, element, data);
            ConfigureToggleInteraction(menu.Profile, element, data);

            return element;
        }

        private static void ConfigureToggleVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuToggleData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var toggle = element.Q<Toggle>("Toggle");
            if (profile.ToggleDataDictionary.TryGetValue(data.Reference, out bool value))
                toggle.value = value;
            else toggle.value = data.Value;
        }

        private static void ConfigureToggleInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuToggleData data)
        {
            var toggle = element.Q<Toggle>("Toggle");
            toggle.RegisterValueChangedCallback(evt =>
            {
                profile.OnToggleValueChanged(data.Reference, evt.newValue);
            });
        }
    }
}