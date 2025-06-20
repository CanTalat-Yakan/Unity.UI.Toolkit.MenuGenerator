using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Style_Spacer_", menuName = "UI/Spacer", order = 3)]
    public class SpacerData : ScriptableObject
    {
        public int Height;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateSpacer(UIMenuGenerator menu, int height)
        {
            var data = ScriptableObject.CreateInstance<SpacerData>();
            data.Height = height;

            return CreateSpacer(menu, data);
        }

        public static VisualElement CreateSpacer(UIMenuGenerator menu, SpacerData data)
        {
            var element = menu.UIGeneratorData.SpacerTemplate.CloneTree();

            ConfigureOptionsVisuals(element, data);

            return element;
        }

        private static void ConfigureOptionsVisuals(VisualElement element, SpacerData data)
        {
            var spacer = element.Q<VisualElement>("Spacer");
            spacer.SetHeight(data.Height);
        }
    }
}