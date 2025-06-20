using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "Style_Header_", menuName = "UI/Header", order = 3)]
    public class HeaderData : ScriptableObject
    {
        public string Text;
    }

    public static partial class UIMenuGeneratorType
    {
        public static VisualElement CreateHeader(UIMenuGenerator menu, HeaderData data)
        {
            var element = menu.Data.HeaderTemplate.CloneTree();

            ConfigureHeaderVisuals(element, data);

            return element;
        }

        private static void ConfigureHeaderVisuals(VisualElement element, HeaderData data)
        {
            var label = element.Q<Label>("Label");
            label.text = data.Text.ToUpper();
        }
    }
}