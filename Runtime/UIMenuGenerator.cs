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

        private UIMenuBreadcrumbDataGenerator _breadcrumbDataGenerator = new();

        [HideInInspector] public UIMenu Menu { get; private set; }
        [HideInInspector] public UIMenuProfile Profile => Menu.Profile;

        public string CurrentCategory { get; private set; }
        public string PreviousCategory { get; private set; }

        public void OnEnable() =>
            Menu = GetComponent<UIMenu>();

        [Button]
        public void Show() =>
            Root?.SetDisplayEnabled(true);

        [Button]
        public void Close() =>
            Root?.SetDisplayEnabled(false);

        public void Populate(bool isRoot, string categoryName, ScriptableObject[] data, Action customDataRedraw = null)
        {
            FetchReferences();

            ClearScrollView();

            if (isRoot)
                ResetCategory();

            if (data != null && data.Length > 0)
                ConfigureRedraw(categoryName, !isRoot, data);
            else
            {
                Redraw = customDataRedraw;
                Redraw?.Invoke();
            }

            _breadcrumbDataGenerator.AddBreadcrumb(this, categoryName, isRoot, data, customDataRedraw);

            UpdateCategoryHistory(categoryName);
            if (data != null && data.Length != 0)
                foreach (var item in data)
                    ProcessDataItem(item);
        }

        private void ProcessDataItem(ScriptableObject data) =>
            RegisterTypeFactory?.Invoke(this, data);

        public void FetchReferences()
        {
            Document ??= GetComponentInChildren<UIDocument>();

            Root ??= Document?.rootVisualElement;

            ScrollView ??= Document?.transform.Find("ScrollView (ScrollView)")?.GetComponent<UIElementLink>();
            Breadcrumbs ??= Document?.transform.Find("GroupBox (Breadcrumbs)")?.GetComponent<UIElementLink>();

            if (Back == null)
            {
                Back ??= Document?.transform.Find("Button (Back)")?.GetComponent<UIElementLink>();
                if (Back?.LinkedElement is Button backButton)
                    backButton.clicked += () => _breadcrumbDataGenerator.GoBackOneBreadcrumb(this);
            }
        }

        private void ConfigureRedraw(string label, bool prefix, ScriptableObject[] data) =>
            Redraw = () =>
            {
                _breadcrumbDataGenerator.ClearBreadcrumbsFromIndex(this, Breadcrumbs.LinkedElement.childCount);
                Populate(prefix, label, data);
            };

        private void UpdateCategoryHistory(string newCategory)
        {
            PreviousCategory = CurrentCategory;
            CurrentCategory = newCategory;
        }

        public void ResetCategory()
        {
            CurrentCategory = null;
            _breadcrumbDataGenerator.ClearBreadcrumbsFromIndex(this);
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
    }
}