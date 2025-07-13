using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [RequireComponent(typeof(UIMenu))]
    public partial class UIMenuGenerator : MonoBehaviour
    {
        [HideInInspector] public UIDocument Document;
        [HideInInspector] public VisualElement Root;
        [HideInInspector] public UIElementLink Back;
        [HideInInspector] public UIElementLink Breadcrumbs;
        [HideInInspector] public UIElementLink ScrollView;

        public Action PopulateRoot;

        private UIMenuBreadcrumbDataGenerator _breadcrumbDataGenerator = new();

        [HideInInspector] public UIMenu Menu { get; private set; }
        [HideInInspector] public UIMenuProfile Profile => Menu.Profile;

        public void OnEnable() =>
            Menu = GetComponent<UIMenu>();

        [ContextMenu("Fetch")]
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

        public Action Redraw;
        private void ConfigureRedraw(string label, bool prefix, ScriptableObject[] data) =>
            Redraw = () =>
            {
                _breadcrumbDataGenerator.ClearBreadcrumbsFromIndex(this, Breadcrumbs.LinkedElement.childCount);
                Populate(prefix, label, data);
            };

        [ContextMenu("Show")]
        [Button]
        public void Show()
        {
            Root?.SetDisplayEnabled(true);
            PopulateRoot?.Invoke();
        }

        [ContextMenu("Close")]
        [Button]
        public void Close()
        {
            Root?.SetDisplayEnabled(false);
        }

        public string CurrentCategory { get; private set; }
        public string PreviousCategory { get; private set; }
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

        public static Action<UIMenuGenerator, ScriptableObject> RegisterTypeFactory;
        private void ProcessDataItem(ScriptableObject data) =>
            RegisterTypeFactory?.Invoke(this, data);
    }
}