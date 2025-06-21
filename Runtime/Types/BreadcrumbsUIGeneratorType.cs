using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static void AddBreadcrumb(UIMenuGenerator menu, string label, bool prefix, ScriptableObject[] data)
        {
            if(menu.Breadcrumbs.LinkedElement is not GroupBox container)
                return;

            var element = menu.Data.BreadcrumbTemplate.CloneTree();

            ConfigureBreadcrumbVisuals(element, label, prefix);
            ConfigureBreadcrumbInteraction(menu, element, container.childCount, prefix, label, data);

            container.Add(element);
        }

        private static void ConfigureBreadcrumbVisuals(VisualElement element, string label, bool prefix)
        {
            var button = element.Q<Button>("Button");
            button.text = (prefix ? "  •  " : string.Empty) + label.ToUpper();
            element.userData = label;
        }

        private static void ConfigureBreadcrumbInteraction(UIMenuGenerator menu, VisualElement element, int index, bool prefix, string label, ScriptableObject[] data)
        {
            var button = element.Q<Button>();
            button.clicked += () =>
            {
                ClearBreadcrumbsFromIndex(menu, index);
                menu.PopulateHierarchy(!prefix, label, data);
            };
        }

        public static void ClearBreadcrumbsFromIndex(UIMenuGenerator menu, int startIndex = 0)
        {
            if (menu.Breadcrumbs.LinkedElement is GroupBox breadcrumbs)
                while (breadcrumbs.childCount > startIndex)
                    breadcrumbs.RemoveAt(breadcrumbs.childCount - 1);
        }
    }
}
