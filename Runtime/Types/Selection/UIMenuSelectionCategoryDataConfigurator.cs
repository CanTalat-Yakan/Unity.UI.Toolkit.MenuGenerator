using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionCategoryDataConfigurator : MonoBehaviour
    {
        public string MenuName;
        public string Reference;

        [Space]
        public int Default;

        private UIMenuSelectionCategoryData _selectionCategoryData;
        [HideInInspector] public UIMenuSelectionCategoryData SelectionCategory => _selectionCategoryData;

        public void Awake()
        {
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetData(Reference, out _selectionCategoryData) ?? false)
            {
                SelectionCategory.IsDynamic = true;
                SelectionCategory.Default = Default;
            }
        }
    }
}
