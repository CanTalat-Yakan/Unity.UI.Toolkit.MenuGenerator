using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEssentials.UIMenuGeneratorType;

namespace UnityEssentials
{
    public partial class UIMenuGenerator : MonoBehaviour
    {
        public UIMenuGeneratorData Data;
        public UIMenuDataProfile Profile;

        public UIDocument Document;
        public UIElementLink Root;
        public UIElementLink Breadcrumbs;
        public UIElementLink ScrollView;

        public string CurrentCategory { get; private set; }
        public string PreviousCategory { get; private set; }

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            Data = ResourceLoader.LoadResource<UIMenuGeneratorData>("UnityEssentials_UIGeneratorData_DefaultUI");
            Data.name = "Default Templates";

            UIMenuDataProfileSerializer.DeserializeData(out Profile, "Example");
            UIMenuDataProfileSerializer.SerializeData(Profile, "Example");
        }

        [ContextMenu("Fetch")]
        public void Fetch()
        {
            Document = GetComponentInChildren<UIDocument>();

            Root = transform.Find("VisualElement (Root)")?.GetComponent<UIElementLink>();
            Breadcrumbs = transform.Find("VisualElement (Breadcrumbs)")?.GetComponent<UIElementLink>();
            ScrollView = transform.Find("VisualElement (ScrollView)")?.GetComponent<UIElementLink>();
        }

        public bool ValidateDependencies() =>
            Data != null && Document != null;
    }

    // UI Management
    public partial class UIMenuGenerator : MonoBehaviour
    {
        public Action Redraw;
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
            var overlay = Data.PopupPanelTemplate.CloneTree();
            overlay.Q<Button>("Label").text = name.ToUpper();
            overlay.Q<Button>("Back").clicked += () => Root.LinkedElement.Remove(overlay);

            return overlay;
        }
    }

    // UI Population
    public partial class UIMenuGenerator : MonoBehaviour
    {
        internal void PopulateHierarchy(bool isRoot, string categoryName, ScriptableObject[] collection)
        {
            if (collection == null || collection.Length == 0) 
                return;

            ClearUI();
            AddBreadcrumb(categoryName, isRoot, collection);

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
        private void AddBreadcrumb(string label, bool excludePrefix, ScriptableObject[] collectionCache)
        {
            if (!(Breadcrumbs.LinkedElement is GroupBox breadcrumbContainer))
                return;

            var breadcrumb = CreateBreadcrumbElement(label, excludePrefix);
            ConfigureBreadcrumbInteraction(breadcrumb, breadcrumbContainer.childCount, excludePrefix, label, collectionCache);

            breadcrumbContainer.Add(breadcrumb);
        }

        private VisualElement CreateBreadcrumbElement(string label, bool excludePrefix)
        {
            var breadcrumb = Data.BreadcrumbTemplate.CloneTree();
            var button = breadcrumb.Q<Button>("Button");
            button.text = (excludePrefix ? string.Empty : "  •  ") + label.ToUpper();
            breadcrumb.userData = label;

            return breadcrumb;
        }

        private void ConfigureBreadcrumbInteraction(VisualElement breadcrumb, int breadcrumbIndex, bool excludePrefix, string originalLabel, ScriptableObject[] collectionCache)
        {
            var button = breadcrumb.Q<Button>();
            button.clicked += Redraw = () =>
            {
                ClearBreadcrumbsFromIndex(breadcrumbIndex);
                PopulateHierarchy(excludePrefix, originalLabel, collectionCache);
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