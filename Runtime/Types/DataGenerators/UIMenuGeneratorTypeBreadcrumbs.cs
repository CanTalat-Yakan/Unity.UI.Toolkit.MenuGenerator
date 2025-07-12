using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public static partial class UIMenuGeneratorType
    {
        public static void AddBreadcrumb(UIMenuDataGenerator menu, string label, bool isRoot, ScriptableObject[] data, Action redraw)
        {
            if (menu.Breadcrumbs.LinkedElement is not GroupBox container)
                return;

            var path = "UIToolkit/UXML/Templates_Default_UI_";
            var name = path + "Breadcrumb_UXML";
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(name).CloneTree();
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
            UIMenuDataGenerator menu,
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
                    menu.Populate(isRoot, label, data, null);
                else
                    menu.Populate(isRoot, label, null, redraw);
            };
        }

        public static void ClearBreadcrumbsFromIndex(UIMenuDataGenerator menu, int startIndex = 0)
        {
            if (menu.Breadcrumbs.LinkedElement is GroupBox breadcrumbs)
                while (breadcrumbs.childCount > startIndex)
                    breadcrumbs.RemoveAt(breadcrumbs.childCount - 1);
        }

        public static void GoBackOneBreadcrumb(UIMenuDataGenerator menu)
        {
            if (menu.Breadcrumbs.LinkedElement is not GroupBox breadcrumbs)
                return;

            if (breadcrumbs.childCount == 0)
                return;

            if (breadcrumbs.childCount == 1)
            {
                menu.Close();
                return;
            }

            var breadcrumbsCount = breadcrumbs.childCount - 1;
            var previousBreadcrumbTemplate = breadcrumbs.ElementAt(breadcrumbsCount - 1);
            foreach (var item in previousBreadcrumbTemplate.Children())
                if (item is Button button)
                    using (var e = new NavigationSubmitEvent() { target = button })
                        button.SendEvent(e);
        }
    }
}
