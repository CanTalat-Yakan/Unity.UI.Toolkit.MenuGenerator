using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "UIGeneratorData", menuName = "Menu/Templates Provider", order = 100)]
    public class UIMenuGeneratorData : ScriptableObject
    {
        [Space]
        // Single-panel settings UI with back navigation, breadcrumb hierarchy, and drill-down structure through nested categories.
        public GameObject HierarchicalMenuTemplate;
        // Fullscreen settings UI with top-level tabs, side navigation, central content, and contextual help.
        public GameObject TabbedMenuTemplate;
        // Dialog-based settings UI with modal windows, header, content, and action buttons are standard structure for modal dialogs..
        public GameObject ModalDialogTemplate;
        // Popup panels for quick settings access, content area.
        public GameObject PopupPanelTemplate;

        [Space]
        public VisualTreeAsset HeaderTemplate;
        public VisualTreeAsset SpacerTemplate;

        [Space]
        public VisualTreeAsset BreadcrumbTemplate;

        [Space]
        public VisualTreeAsset CategoryTemplate;
        public VisualTreeAsset ButtonTemplate;

        [Space]
        public VisualTreeAsset InputTemplate;

        [Space]
        public VisualTreeAsset OptionsTemplate;

        [Space]
        public VisualTreeAsset SliderTemplate;
        public VisualTreeAsset SliderIntTemplate;

        [Space]
        public VisualTreeAsset ToggleTemplate;

        [Space]
        public VisualTreeAsset SelectionCategoryTemplate;
        public VisualTreeAsset SelectionTileTemplate;

        [Space]
        public VisualTreeAsset ColorPickerTemplate;
        public VisualTreeAsset ColorSliderTemplate;
    }
}