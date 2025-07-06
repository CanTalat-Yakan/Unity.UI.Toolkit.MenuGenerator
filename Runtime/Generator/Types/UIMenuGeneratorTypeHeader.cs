using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateHeader(UIMenuGenerator menu, UIMenuHeaderData data)
        {
            var element = menu.Data.HeaderTemplate.CloneTree();
            ConfigureHeaderVisuals(element, data);
            return element;
        }

        private static void ConfigureHeaderVisuals(VisualElement element, UIMenuHeaderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Name.ToUpper();
            label.style.marginTop = data.MarginTop;
        }
    }
}