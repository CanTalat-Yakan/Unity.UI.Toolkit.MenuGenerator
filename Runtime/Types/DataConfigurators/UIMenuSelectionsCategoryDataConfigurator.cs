using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionsCategoryDataConfigurator : MonoBehaviour
    {
        public UIMenuData MenuData;
        public string Reference;

        [Space]
        public int Default;

        private UIMenuSelectionCategoryData _selectionCategory;
        [HideInInspector] public UIMenuSelectionCategoryData SelectionCategory => _selectionCategory;

        public void Awake()
        {
            if (MenuData?.GetDataByReference(Reference, out _selectionCategory) ?? false)
            {
                SelectionCategory.IsDynamic = true;
                SelectionCategory.Default = Default;
            }
        }
    }
}
