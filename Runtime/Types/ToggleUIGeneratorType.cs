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

            profile.Toggles.TryGetValue(data.Reference, data.Default, out var value);
            
            var toggle = element.Q<Toggle>("Toggle");
            toggle.value = value;
        }

        private static void ConfigureToggleInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuToggleData data)
        {
            var toggle = element.Q<Toggle>("Toggle");
            toggle.RegisterValueChangedCallback(e =>
            {
                profile.OnToggleValueChanged(data.Reference, e.newValue);
            });
        }
    }
}