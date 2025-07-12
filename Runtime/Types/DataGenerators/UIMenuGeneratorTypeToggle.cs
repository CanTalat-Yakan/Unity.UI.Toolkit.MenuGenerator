using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateToggle(UIMenuDataGenerator menu, UIMenuToggleData data)
        {
            var path = "UIToolkit/UXML/Templates_Types_UI_";
            var name = path + "Toggle_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
            ConfigureToggleVisuals(menu.Profile, element, data);
            ConfigureToggleInteraction(menu.Profile, element, data);
            return element;
        }

        private static void ConfigureToggleVisuals(UIMenuDataProfile profile, VisualElement element, UIMenuToggleData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();

            var value = profile.GetData(data.Reference, data.Default);
            
            var toggle = element.Q<Toggle>("Toggle");
            toggle.value = value;
        }

        private static void ConfigureToggleInteraction(UIMenuDataProfile profile, VisualElement element, UIMenuToggleData data)
        {
            var toggle = element.Q<Toggle>("Toggle");
            toggle.RegisterValueChangedCallback((evt) =>
            {
                profile.OnToggleValueChanged(data.Reference, evt.newValue);
            });
        }
    }
}