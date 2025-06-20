using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEssentials.UIMenuGeneratorType;

namespace UnityEssentials
{
    public partial class UIMenuGenerator : MonoBehaviour
    {
        [Header("Data Configuration")]
        public UIMenuGeneratorData UIGeneratorData;

        [Space]
        public UIMenuDataProfile Profile;

        [HideInInspector] public UIDocument Document;
        [HideInInspector] public UIElementLink Root;
        [HideInInspector] public UIElementLink Breadcrumbs;
        [HideInInspector] public UIElementLink ScrollView;

        public string CurrentCategory { get; private set; }
        public string PreviousCategory { get; private set; }

        public Action Redraw;

        public void Initialize()
        {
            Cleanup();
            InitializeDocument();
            InitializeData();
        }

        public void Cleanup()
        {
            DestroyAllChildren();
            UIGeneratorData = null;
            Document = null;
            Root = null;
        }

        private void DestroyAllChildren()
        {
            while (transform.childCount > 0)
                if (Application.isEditor)
                    DestroyImmediate(transform.GetChild(0).gameObject);
                else Destroy(transform.GetChild(0).gameObject);
        }

        public void InitializeDocument()
        {
            var prefab = ResourceLoader.InstantiatePrefab("UnityEssentials_Prefab_DefaultUI", "UI Document", transform);
            if (prefab != null)
            {
                Document = prefab.GetComponent<UIDocument>();
                Root = prefab.transform.Find("VisualElement (Root)")?.GetComponent<UIElementLink>();
            }
        }

        public void InitializeData(UIMenuGeneratorData data = null)
        {
            UIGeneratorData = data ??= ResourceLoader.LoadResource<UIMenuGeneratorData>("UnityEssentials_UIGeneratorData_DefaultUI");
        }

        public bool ValidateDependencies() =>
            UIGeneratorData != null &&
            Breadcrumbs != null &&
            ScrollView != null &&
            Root != null;
    }

    // UI Management
    public partial class UIMenuGenerator : MonoBehaviour
    {
        public void RedrawUI() =>
            Redraw?.Invoke();

        public void ResetUI()
        {
            CurrentCategory = null;
            ClearBreadcrumbsFromIndex(0);
        }

        public void ClearUI()
        {
            if (ScrollView.LinkedElement is ScrollView scrollView)
                scrollView.Clear();
        }

        public void AddElementToScrollView(VisualElement element)
        {
            if(element == null) 
                return;

            if (ScrollView.LinkedElement is ScrollView scrollView)
                scrollView.Add(element);
        }

        public void AddElementToRoot(VisualElement element)
        {
            if (element == null)
                return;

            if (Root.LinkedElement is VisualElement root)
                root.Add(element);
        }

        public VisualElement CreatePopup(string name)
        {
            var overlay = UIGeneratorData.PopupPanelTemplate.CloneTree();
            overlay.Q<Button>("Label").text = name.ToUpper();
            overlay.Q<Button>("Back").clicked += () => Root.LinkedElement.Remove(overlay);

            return overlay;
        }
    }

    // UI Population
    public partial class UIMenuGenerator : MonoBehaviour
    {
        internal void PopulateHierarchy(string categoryName, bool isSubCategory, ScriptableObject[] collection)
        {
            if (collection == null || collection.Length == 0) return;

            ClearUI();
            AddBreadcrumb(categoryName, isSubCategory, collection);

            UpdateCategoryHistory(categoryName);

            foreach (var data in collection)
                ProcessDataItem(data);
        }

        private void UpdateCategoryHistory(string newCategory)
        {
            PreviousCategory = CurrentCategory;
            CurrentCategory = newCategory;
        }

        private void ProcessDataItem(ScriptableObject data)
        {
            var element = data switch
            {
                CategoryData category => CreateCategory(this, category),
                HeaderData header => CreateHeader(this, header),
                SpacerData spacer => CreateSpacer(this, spacer),
                ButtonData button => CreateButton(this, button),
                ToggleData toggle => CreateToggle(this, toggle),
                InputData input => CreateInput(this, input),
                OptionsData options => CreateOptions(this, options),
                SliderData slider => CreateSlider(this, slider),
                SelectionDataCollectionGroup selectionCategory => CreateSelectionCategory(this, selectionCategory),
                ColorPickerDataGroup colorCategory => CreateColorPickerButton(this, colorCategory),
                ColorSliderData colorSliderData => CreateColorSlider(this, colorSliderData),
                _ => null
            };

            AddElementToScrollView(element);
        }
    }

    // Breadcrumb Configuration
    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddBreadcrumb(string label, bool includePrefix, ScriptableObject[] collectionCache)
        {
            if (!(Breadcrumbs.LinkedElement is GroupBox breadcrumbContainer))
                return;

            var breadcrumb = CreateBreadcrumbElement(label, includePrefix);
            ConfigureBreadcrumbInteraction(breadcrumb, breadcrumbContainer.childCount, includePrefix, label, collectionCache);

            breadcrumbContainer.Add(breadcrumb);
        }

        private VisualElement CreateBreadcrumbElement(string label, bool includePrefix)
        {
            var breadcrumb = UIGeneratorData.BreadcrumbTemplate.CloneTree();
            var button = breadcrumb.Q<Button>("Button");
            button.text = (includePrefix ? "  •  " : "") + label.ToUpper();
            breadcrumb.userData = label;

            return breadcrumb;
        }

        private void ConfigureBreadcrumbInteraction(VisualElement breadcrumb, int breadcrumbIndex, bool includePrefix, string originalLabel, ScriptableObject[] collectionCache)
        {
            var button = breadcrumb.Q<Button>();
            button.clicked += Redraw = () =>
            {
                ClearBreadcrumbsFromIndex(breadcrumbIndex);
                PopulateHierarchy(originalLabel, includePrefix, collectionCache);
            };
        }

        private void ClearBreadcrumbsFromIndex(int startIndex)
        {
            if (Breadcrumbs.LinkedElement is GroupBox groupBox)
                while (groupBox.childCount > startIndex)
                    groupBox.RemoveAt(groupBox.childCount - 1);
        }
    }
}