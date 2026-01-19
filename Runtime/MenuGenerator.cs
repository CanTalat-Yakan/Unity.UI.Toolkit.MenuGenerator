using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public partial class MenuGenerator : MonoBehaviour
    {
        [Info]
        [SerializeField]
        private string _info =
            "UIMenuGenerator is responsible for dynamically building and managing UI menus using Unity UI Toolkit. " +
            "It handles the instantiation, population, and navigation of menu elements, " +
            "to enable flexible, data-driven menu creation and runtime updates.";

        [HideInInspector] public UIDocument Document;
        [HideInInspector] public VisualElement Root;
        [HideInInspector] public UIElementLink Overlay;
        [HideInInspector] public UIElementLink Breadcrumbs;
        [HideInInspector] public UIElementLink ScrollView;
        [HideInInspector] public UIElementLink Back;

        public static Action<MenuGenerator, ScriptableObject> RegisterTypeFactory;
        public Action PopulateRoot;
        public Action Redraw;

        public MenuBreadcrumbDataGenerator BreadcrumbDataGenerator = new();

        [HideInInspector] public Menu Menu => _menu ??= this.GetOrAddComponent<Menu>();
        [NonSerialized] private Menu _menu;

        [HideInInspector] public SettingsProfile Profile => Menu.ProfileManager.GetCurrentProfile();

        [Button]
        public void Show()
        {
            Fetch();
            ClearBreadcrumbs();
            Root?.SetDisplayEnabled(true);
            Overlay?.LinkedElement?.SetVisibility(true);
            PopulateRoot?.Invoke();
        }

        [Button]
        public void Close()
        {
            Fetch();
            ClearBreadcrumbs();
            Root?.SetDisplayEnabled(false);
            Overlay?.LinkedElement?.SetVisibility(false);
        }

        public void Fetch()
        {
            Document ??= GetComponentInChildren<UIDocument>();

            Root ??= Document?.rootVisualElement;
            Overlay ??= Document?.transform.Find("GroupBox (Overlay)")?.GetComponent<UIElementLink>();
            Breadcrumbs ??= Document?.transform.Find("GroupBox (Breadcrumbs)")?.GetComponent<UIElementLink>();
            ScrollView ??= Document?.transform.Find("ScrollView (ScrollView)")?.GetComponent<UIElementLink>();

            if (Back == null)
            {
                Back ??= Document?.transform.Find("Button (Back)")?.GetComponent<UIElementLink>();
                if (Back?.LinkedElement is Button backButton)
                    backButton.clicked += () => MenuBreadcrumbDataGenerator.NavigateBack(this);
            }
        }

        public void Populate(bool isRoot, string label, ScriptableObject[] data, Action customDataDrawCall = null)
        {
            ClearScrollView();

            ConfigureRedraw(isRoot, label, data, customDataDrawCall);

            BreadcrumbDataGenerator.AddBreadcrumb(this, isRoot, label, Redraw);

            if (data != null && data.Length != 0)
                foreach (var typeData in data)
                    RegisterTypeFactory?.Invoke(this, typeData);

            customDataDrawCall?.Invoke();
        }

        public void ClearBreadcrumbs() =>
            MenuBreadcrumbDataGenerator.ClearFromIndex(this, 0);

        public void ClearScrollView()
        {
            if (ScrollView.LinkedElement is ScrollView scrollView)
                scrollView.Clear();
        }

        public void AddToScrollView(VisualElement element)
        {
            if (element == null)
                return;

            if (ScrollView.LinkedElement is ScrollView scrollView)
                scrollView.Add(element);
        }

        public void AddToRoot(VisualElement element)
        {
            if (element == null)
                return;

            Root.Add(element);
        }

        private void ConfigureRedraw(bool isRoot, string label, ScriptableObject[] data, Action customDataDrawCall)
        {
            var breadcrumbCount = Breadcrumbs.LinkedElement.childCount;
            Redraw = () =>
            {
                MenuBreadcrumbDataGenerator.ClearFromIndex(this, breadcrumbCount);
                Populate(isRoot, label, data, customDataDrawCall);
            };
        }
    }
}