using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuToggleData : UIGeneratorTypeTemplate
    {
        [Space]
        public bool Default;
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

            if (!profile.ToggleDataDictionary.TryGetValue(data.Reference, out bool value))
                value = data.Default;

            var toggle = element.Q<Toggle>("Toggle");
            toggle.value = value;
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