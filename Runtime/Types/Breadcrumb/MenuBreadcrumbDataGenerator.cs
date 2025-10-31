using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuBreadcrumbGeneratorData : IUIMenuTypeData
    {
        public Action Redraw;

        [Space]
        public bool IsRoot;
        public string Label;
        public int Index;
    }

    public class MenuBreadcrumbDataGenerator : MenuTypeDataGeneratorBase<UIMenuBreadcrumbGeneratorData>
    {
        public void AddBreadcrumb(MenuGenerator menu,bool isRoot, string label,  Action redraw)
        {
            if (menu.Breadcrumbs.LinkedElement is not GroupBox container)
                return;

            var breadcrumbData = new UIMenuBreadcrumbGeneratorData
            {
                IsRoot = isRoot,
                Label = label,
                Redraw = redraw,
                Index = container.childCount
            };

            var element = CreateElement(menu, breadcrumbData);
            container.Add(element);
        }

        public static readonly string ResourcePath = Path + "Breadcrumb_UXML";
        public override VisualElement CreateElement(MenuGenerator menu, UIMenuBreadcrumbGeneratorData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(MenuGenerator menu, VisualElement element, UIMenuBreadcrumbGeneratorData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Label;

            if (data.IsRoot)
                button.iconImage = null;
        }

        public override void ConfigureInteraction(MenuGenerator menu, VisualElement element, UIMenuBreadcrumbGeneratorData data)
        {
            var button = element.Q<Button>();
            button.clicked += () =>
            {
                ClearFromIndex(menu, data.Index);
                data.Redraw?.Invoke();
            };
        }

        public static void ClearFromIndex(MenuGenerator menu, int startIndex)
        {
            if (menu.Breadcrumbs.LinkedElement is GroupBox container)
                while (container.childCount > startIndex)
                    container.RemoveAt(container.childCount - 1);
        }

        public static void NavigateBack(MenuGenerator menu)
        {
            if (menu.Breadcrumbs.LinkedElement is not GroupBox container)
                return;

            if (container.childCount == 0)
                return;

            if (container.childCount == 1)
            {
                menu.Close();
                return;
            }

            var breadcrumbsCount = container.childCount - 1;
            var previousBreadcrumbTemplate = container.ElementAt(breadcrumbsCount - 1);
            foreach (var item in previousBreadcrumbTemplate.Children())
                if (item is Button button)
                    using (var evt = new NavigationSubmitEvent() { target = button })
                        button.SendEvent(evt);
        }
    }
}
