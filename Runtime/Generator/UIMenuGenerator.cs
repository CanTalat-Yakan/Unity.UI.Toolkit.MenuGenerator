using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public partial class UIMenuGenerator : MonoBehaviour
    {
        [Header("Data Configuration")]
        public UIMenuGeneratorData UIGeneratorData;

        [Space]
        public DataProfile Profile;

        [Header("UI Containers")]
        [SerializeField] private UIElementLink _breadcrumbs;
        [SerializeField] private UIElementLink _scrollView;
        [SerializeField] private UIElementLink _root;

        public string CurrentCategory { get; private set; }
        public string PreviousCategory { get; private set; }

        private Action _redraw;

        public bool ValidateDependencies() =>
            UIGeneratorData != null &&
            _breadcrumbs != null &&
            _scrollView != null &&
            _root != null;
    }

    // UI Management
    public partial class UIMenuGenerator : MonoBehaviour
    {
        public void RedrawUI()
        {
            _redraw?.Invoke();
        }

        internal void ResetUI()
        {
            CurrentCategory = null;
            ClearBreadcrumbsFromIndex(0);
        }

        private void ClearUI()
        {
            if (_scrollView.LinkedElement is ScrollView scrollView)
                scrollView.Clear();
        }

        private void AddElementToScrollView(VisualElement element)
        {
            if (_scrollView.LinkedElement is ScrollView scrollView)
                scrollView.Add(element);
        }

        private VisualElement CreateOverlay(string name)
        {
            var overlay = UIGeneratorData.OverlayLeftPaneTemplate.CloneTree();
            overlay.Q<Button>("Label").text = name.ToUpper();
            overlay.Q<Button>("Back").clicked += () => _root.LinkedElement.Remove(overlay);

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

            AddSpacer(0);
        }

        private void UpdateCategoryHistory(string newCategory)
        {
            PreviousCategory = CurrentCategory;
            CurrentCategory = newCategory;
        }

        private void ProcessDataItem(ScriptableObject data)
        {
            switch (data)
            {
                case CategoryData category:
                    AddCategory(category);
                    break;

                case HeaderData header:
                    AddHeader(header);
                    break;
                case SpacerData spacer:
                    AddSpacer(spacer);
                    break;

                case ButtonData button:
                    AddButton(button);
                    break;

                case ToggleData toggle:
                    AddToggle(toggle);
                    break;
                    
                case InputData input:
                    AddInput(input);
                    break;

                case OptionsData options:
                    AddOptions(options);
                    break;
                    
                case SliderData slider:
                    AddSlider(slider);
                    break;

                case SelectionDataCollectionGroup selectionCategory:
                    AddSelectionCategory(selectionCategory);
                    break;

                case ColorPickerDataGroup colorCategory:
                    AddColorPickerButton(colorCategory);
                    break;
                case ColorSliderData colorSliderData:
                    AddColorSlider(colorSliderData);
                    break;
            }
        }
    }

    // Breadcrumb Configuration
    public partial class UIMenuGenerator : MonoBehaviour
    {
        private void AddBreadcrumb(string label, bool includePrefix, ScriptableObject[] collectionCache)
        {
            if (!(_breadcrumbs.LinkedElement is GroupBox breadcrumbContainer))
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
            button.clicked += _redraw = () =>
            {
                ClearBreadcrumbsFromIndex(breadcrumbIndex);
                PopulateHierarchy(originalLabel, includePrefix, collectionCache);
            };
        }

        private void ClearBreadcrumbsFromIndex(int startIndex)
        {
            if (_breadcrumbs.LinkedElement is GroupBox groupBox)
                while (groupBox.childCount > startIndex)
                    groupBox.RemoveAt(groupBox.childCount - 1);
        }
    }
}