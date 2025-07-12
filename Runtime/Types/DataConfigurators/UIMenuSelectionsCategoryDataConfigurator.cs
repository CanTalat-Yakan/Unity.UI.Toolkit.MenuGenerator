using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionsCategoryDataConfigurator : MonoBehaviour
    {
        public string MenuName;
        public string Reference;

        [Space]
        public int Default;

        private UIMenuSelectionCategoryData _selectionCategory;
        [HideInInspector] public UIMenuSelectionCategoryData SelectionCategory => _selectionCategory;

        public void Awake()
        {
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetDataByReference(Reference, out _selectionCategory) ?? false)
            {
                SelectionCategory.IsDynamic = true;
                SelectionCategory.Default = Default;
            }
        }
    }
}
