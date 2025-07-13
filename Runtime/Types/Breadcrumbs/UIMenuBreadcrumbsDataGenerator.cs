using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public class UIMenuBreadcrumbGeneratorData : UIMenuTypeBase
    {
        public ScriptableObject[] Data;
        public Action Redraw;

        [Space]
        public string Label;
        public bool IsRoot;
        public int Index;
    }

    public class UIMenuBreadcrumbDataGenerator : UIMenuGeneratorTypeBase<UIMenuBreadcrumbGeneratorData>
    {
        public void AddBreadcrumb(UIMenuDataGenerator menu, string label, bool isRoot, ScriptableObject[] data, Action redraw)
        {
            if (menu.Breadcrumbs.LinkedElement is not GroupBox container)
                return;

            var breadcrumbData = new UIMenuBreadcrumbGeneratorData
            {
                Label = label,
                IsRoot = isRoot,
                Data = data,
                Redraw = redraw,
                Index = container.childCount
            };

            var element = CreateElement(menu, breadcrumbData);
            container.Add(element);
        }

        public static readonly string ResourcePath = Path + "Breadcrumb_UXML";
        public override VisualElement CreateElement(UIMenuDataGenerator menu, UIMenuBreadcrumbGeneratorData data)
        {
            var element = ResourceLoader.LoadResource<VisualTreeAsset>(ResourcePath).CloneTree();
            ConfigureVisuals(menu, element, data);
            ConfigureInteraction(menu, element, data);
            return element;
        }

        public override void ConfigureVisuals(UIMenuDataGenerator menu, VisualElement element, UIMenuBreadcrumbGeneratorData data)
        {
            var button = element.Q<Button>("Button");
            button.text = data.Label.ToUpper();

            if (data.IsRoot)
                button.iconImage = null;
        }

        public override void ConfigureInteraction(UIMenuDataGenerator menu, VisualElement element, UIMenuBreadcrumbGeneratorData data)
        {
            var button = element.Q<Button>();
            button.clicked += () =>
            {
                ClearBreadcrumbsFromIndex(menu, data.Index);
                if (data != null && data.Data.Length > 0)
                    menu.Populate(data.IsRoot, data.Label, data.Data, null);
                else
                    menu.Populate(data.IsRoot, data.Label, null, data.Redraw);
            };
        }

        public void ClearBreadcrumbsFromIndex(UIMenuDataGenerator menu, int startIndex = 0)
        {
            if (menu.Breadcrumbs.LinkedElement is GroupBox container)
                while (container.childCount > startIndex)
                    container.RemoveAt(container.childCount - 1);
        }

        public void GoBackOneBreadcrumb(UIMenuDataGenerator menu)
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
