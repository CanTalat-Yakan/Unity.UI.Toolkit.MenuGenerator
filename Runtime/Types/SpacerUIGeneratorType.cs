using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuSpacerData : UIGeneratorTypeTemplate
    {
        public int Height = 20;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSpacer(UIMenuGenerator menu, int height)
        {
            var data = ScriptableObject.CreateInstance<UIMenuSpacerData>();
            data.Height = height;

            return CreateSpacer(menu, data);
        }

        public static VisualElement CreateSpacer(UIMenuGenerator menu, UIMenuSpacerData data)
        {
            var element = menu.Data.SpacerTemplate.CloneTree();

            ConfigureOptionsVisuals(element, data);

            return element;
        }

        private static void ConfigureOptionsVisuals(VisualElement element, UIMenuSpacerData data)
        {
            var spacer = element.Q<VisualElement>("Spacer");
            spacer.SetHeight(data.Height);
        }
    }
}