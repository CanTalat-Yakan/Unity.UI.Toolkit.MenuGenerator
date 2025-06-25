using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public partial class UIMenuGenerator : MonoBehaviour
    {
        public UIMenuGeneratorData Data;
        public UIMenuDataProfile Profile;

        [HideInInspector] public UIDocument Document;
        [HideInInspector] public UIElementLink Root;
        [HideInInspector] public UIElementLink Breadcrumbs;
        [HideInInspector] public UIElementLink ScrollView;

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            Data = ResourceLoader.LoadResource<UIMenuGeneratorData>("UnityEssentials_UIGeneratorData_DefaultUI");
            Data.name = "Default Templates";
        }

        [ContextMenu("Fetch")]
        public void Fetch()
        {
            Document = GetComponentInChildren<UIDocument>();

            Root = Document.transform.Find("VisualElement (Root)")?.GetComponent<UIElementLink>();
            Breadcrumbs = Document.transform.Find("VisualElement (Breadcrumbs)")?.GetComponent<UIElementLink>();
            ScrollView = Document.transform.Find("VisualElement (ScrollView)")?.GetComponent<UIElementLink>();
        }

        public Action Redraw;
        private void ConfigureRedraw(string label, bool prefix, ScriptableObject[] data) =>
            Redraw = () =>
            {
                UIMenuGeneratorType.ClearBreadcrumbsFromIndex(this, Breadcrumbs.LinkedElement.childCount);
                PopulateHierarchy(prefix, label, data);
            };

        public bool ValidateDependencies() =>
            Data != null && Document != null;
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
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
            UIMenuGeneratorType.ClearBreadcrumbsFromIndex(this);
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

            if (Root.LinkedElement is VisualElement root)
                root.Add(element);
        }

        public VisualElement CreatePopup(string name)
        {
            var popup = Instantiate(Data.PopupPanelTemplate);
            var root = popup.GetComponent<UIDocument>().rootVisualElement;
            root.Q<Button>("Label").text = name.ToUpper();
            root.Q<Button>("Back").clicked += () => Root.LinkedElement.Remove(root);
            return root;
        }
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        public void PopulateHierarchy(bool isRoot, string categoryName, ScriptableObject[] collection)
        {
            if (collection == null || collection.Length == 0)
                return;

            ClearScrollView();

            ConfigureRedraw(categoryName, !isRoot, collection);
            UIMenuGeneratorType.AddBreadcrumb(this, categoryName, !isRoot, collection);

            UpdateCategoryHistory(categoryName);

            foreach (var data in collection)
                ProcessDataItem(data);
        }

        private void ProcessDataItem(ScriptableObject data) =>
            AddElementToScrollView(data switch
            {
                UIMenuCategoryData category => UIMenuGeneratorType.CreateCategory(this, category),
                UIMenuHeaderData header => UIMenuGeneratorType.CreateHeader(this, header),
                UIMenuSpacerData spacer => UIMenuGeneratorType.CreateSpacer(this, spacer),
                UIMenuButtonData button => UIMenuGeneratorType.CreateButton(this, button),
                UIMenuToggleData toggle => UIMenuGeneratorType.CreateToggle(this, toggle),
                UIMenuInputData input => UIMenuGeneratorType.CreateInput(this, input),
                UIMenuOptionsData options => UIMenuGeneratorType.CreateOptions(this, options),
                UIMenuSliderData slider => UIMenuGeneratorType.CreateSlider(this, slider),
                UIMenuSelectionDataCategory selectionCategory => UIMenuGeneratorType.CreateSelectionCategory(this, selectionCategory),
                UIMenuColorPickerDataGroup colorCategory => UIMenuGeneratorType.CreateColorPickerButton(this, colorCategory),
                UIMenuColorSliderData colorSliderData => UIMenuGeneratorType.CreateColorSlider(this, colorSliderData),
                _ => null
            });
    }
}