using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuHeaderData : ScriptableObject
    {
        public string Name;

        public UIMenuHeaderData SetName(string name, string uniqueName = null)
        {
            uniqueName ??= name;
            base.name = uniqueName;
            Name = name;
            return this;
        }
    }

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
        }
    }
}