using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static void AddBreadcrumb(UIMenuGenerator menu, string label, bool isRoot, ScriptableObject[] data, Action redraw)
        {
            if (menu.Breadcrumbs.LinkedElement is not GroupBox container)
                return;

            var element = menu.Data.BreadcrumbTemplate.CloneTree();
            ConfigureBreadcrumbVisuals(element, label, !isRoot);
            ConfigureBreadcrumbInteraction(menu, element, container.childCount, isRoot, label, data, redraw);
            container.Add(element);
        }

        private static void ConfigureBreadcrumbVisuals(VisualElement element, string label, bool showIcon)
        {
            var button = element.Q<Button>("Button");
            button.text = label.ToUpper();

            if (!showIcon)
                button.iconImage = null;
        }

        private static void ConfigureBreadcrumbInteraction(
            UIMenuGenerator menu,
            VisualElement element,
            int index, bool isRoot, string label,
            ScriptableObject[] data,
            Action redraw)
        {
            var button = element.Q<Button>();
            button.clicked += () =>
            {
                ClearBreadcrumbsFromIndex(menu, index);
                if (data != null && data.Length > 0)
                    menu.PopulateHierarchy(isRoot, label, data, null);
                else
                    menu.PopulateHierarchy(isRoot, label, null, redraw);
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
