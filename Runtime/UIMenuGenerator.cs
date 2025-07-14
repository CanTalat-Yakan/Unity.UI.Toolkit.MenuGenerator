using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public partial class UIMenuGenerator : MonoBehaviour
    {
        [Info]
        [SerializeField]
        private string _info =
            "UIMenuGenerator is responsible for dynamically building and managing UI menus using Unity UI Toolkit. " +
            "It handles the instantiation, population, and navigation of menu elements, " +
            "to enable flexible, data-driven menu creation and runtime updates.";

        [HideInInspector] public UIDocument Document;
        [HideInInspector] public VisualElement Root;
        [HideInInspector] public UIElementLink Back;
        [HideInInspector] public UIElementLink Breadcrumbs;
        [HideInInspector] public UIElementLink ScrollView;

        public static Action<UIMenuGenerator, ScriptableObject> RegisterTypeFactory;
        public Action PopulateRoot;
        public Action Redraw;

        public UIMenuBreadcrumbDataGenerator BreadcrumbDataGenerator = new();

        [HideInInspector] public UIMenu Menu => _menu ??= this.GetOrAddComponent<UIMenu>();
        [NonSerialized] private UIMenu _menu;

        [HideInInspector] public UIMenuProfile Profile => Menu.Profile;

        [Button]
        public void Show()
        {
            Fetch();
            UIMenuBreadcrumbDataGenerator.ClearFromIndex(this, 0);
            Root?.SetDisplayEnabled(true);
            PopulateRoot?.Invoke();
        }

        [Button]
        public void Close()
        {
            Fetch();
            UIMenuBreadcrumbDataGenerator.ClearFromIndex(this, 0);
            Root?.SetDisplayEnabled(false);
        }

        public void Fetch()
        {
            Document ??= GetComponentInChildren<UIDocument>();

            Root ??= Document?.rootVisualElement;

            ScrollView ??= Document?.transform.Find("ScrollView (ScrollView)")?.GetComponent<UIElementLink>();
            Breadcrumbs ??= Document?.transform.Find("GroupBox (Breadcrumbs)")?.GetComponent<UIElementLink>();

            if (Back == null)
            {
                Back ??= Document?.transform.Find("Button (Back)")?.GetComponent<UIElementLink>();
                if (Back?.LinkedElement is Button backButton)
                    backButton.clicked += () => UIMenuBreadcrumbDataGenerator.NavigateBack(this);
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

        public void ClearScrollView()
        {
            if (ScrollView.LinkedElement is ScrollView scrollView)
                scrollView.Clear();
        }

        public void AddElementToScrollView(VisualElement element)
        {
            if (element == null)
                return;

            if (ScrollView.LinkedElement is ScrollView scrollView)
                scrollView.Add(element);
        }

        public void AddElementToRoot(VisualElement element)
        {
            if (element == null)
                return;

            Root.Add(element);
        }

        private void ConfigureRedraw(bool isRoot, string label, ScriptableObject[] data, Action customDataDrawCall) =>
            Redraw = () =>
            {
                UIMenuBreadcrumbDataGenerator.ClearFromIndex(this, Breadcrumbs.LinkedElement.childCount);
                Populate(isRoot, label, data, customDataDrawCall);
            };
    }
}