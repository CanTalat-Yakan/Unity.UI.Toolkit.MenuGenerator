using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEssentials
{
    [CreateAssetMenu(fileName = "UIGeneratorData", menuName = "UI/Templates Provider", order = 1)]
    public class UIMenuGeneratorData : ScriptableObject
    {
        [Space]
        public VisualTreeAsset OverlayTemplate;

        [Space]
        public VisualTreeAsset BreadcrumbTemplate;

        [Space]
        public VisualTreeAsset HeaderTemplate;
        public VisualTreeAsset SpacerTemplate;

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