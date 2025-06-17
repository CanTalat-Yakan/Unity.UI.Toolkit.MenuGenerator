using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "UIGeneratorData", menuName = "UI/Templates Provider", order = 1)]
    public class UIMenuGeneratorData : ScriptableObject
    {
        [Space]
        // Fullscreen settings UI with top-level tabs, side navigation, central content, and contextual help.
        public VisualTreeAsset TabbedMenuTemplate;
        // Single-panel settings UI with back navigation, breadcrumb hierarchy, and drill-down structure through nested categories.
        public VisualTreeAsset HierarchicalMenuTemplate;
        // Dialog-based settings UI with modal windows, header, content, and action buttons are standard structure for modal dialogs..
        public VisualTreeAsset ModelDialogTemplate;
        // Popup panels for quick settings access, content area.
        public VisualTreeAsset PopupPanelTemplate;

        [Space]
        public VisualTreeAsset HeaderTemplate;
        public VisualTreeAsset SpacerTemplate;

        [Space]
        public VisualTreeAsset BreadcrumbTemplate;

        [Space]
        public VisualTreeAsset CategoryTemplate;

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