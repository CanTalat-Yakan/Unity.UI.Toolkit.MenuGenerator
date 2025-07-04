using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    public partial class UIMenuGenerator : MonoBehaviour
    {
        public UIMenuGeneratorData Data;

        [Space]
        public UIMenuDataProfile Profile;
        public UIMenuDataProfile Default;

        [HideInInspector] public UIDocument Document;
        [HideInInspector] public VisualElement Root;
        [HideInInspector] public UIElementLink Back;
        [HideInInspector] public UIElementLink Breadcrumbs;
        [HideInInspector] public UIElementLink ScrollView;

        [ContextMenu("Initialize")]
        public void Initialize()
        {
            Data = ResourceLoader.LoadResource<UIMenuGeneratorData>("UnityEssentials_UIGeneratorData_DefaultUI");
            Data.name = "Default Templates";
        }

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
                    backButton.clicked += () => UIMenuGeneratorType.GoBackOneBreadcrumb(this);
            }
        }

        public Action Redraw;
        private void ConfigureRedraw(string label, bool prefix, ScriptableObject[] data) =>
            Redraw = () =>
            {
                UIMenuGeneratorType.ClearBreadcrumbsFromIndex(this, Breadcrumbs.LinkedElement.childCount);
                Populate(prefix, label, data);
            };

        public bool ValidateDependencies() =>
            Data != null && Document != null;
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        [ContextMenu("Show")]
        public void Show()
        {
        }

        [ContextMenu("Close")]
        public void Close()
        {
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

            Root.Add(element);
        }

        public VisualElement CreatePopup(string name)
        {
            var popup = Instantiate(Data.PopupPanelTemplate);
            var root = popup.GetComponent<UIDocument>().rootVisualElement;
            root.Q<Button>("Label").text = name.ToUpper();
            root.Q<Button>("Back").clicked += () => Root.Remove(root);
            return root;
        }
    }

    public partial class UIMenuGenerator : MonoBehaviour
    {
        public void Populate(bool isRoot, string categoryName, ScriptableObject[] data, Action redraw = null)
        {
            FetchReferences();

            ClearScrollView();

            if (data != null && data.Length > 0)
                ConfigureRedraw(categoryName, !isRoot, data);
            else
            {
                Redraw = redraw;
                Redraw?.Invoke();
            }

            UIMenuGeneratorType.AddBreadcrumb(this, categoryName, isRoot, data, redraw);

            UpdateCategoryHistory(categoryName);

            if (data != null && data.Length != 0)
                foreach (var item in data)
                    ProcessDataItem(item);
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
                UIMenuColorPickerData colorCategory => UIMenuGeneratorType.CreateColorPickerButton(this, colorCategory),
                UIMenuColorSliderData colorSliderData => UIMenuGeneratorType.CreateColorSlider(this, colorSliderData),
                _ => null
            });
    }
}