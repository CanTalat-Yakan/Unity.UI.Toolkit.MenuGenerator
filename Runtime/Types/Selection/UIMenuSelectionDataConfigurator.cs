using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionDataConfigurator : MonoBehaviour
    {
        public string MenuName;
        public string Reference;

        [Space]
        public bool Reverse;
        public UIMenuSelectionData Selections;

        private UIMenuSelectionGroupData _selectionGroupData;
        [HideInInspector] public UIMenuSelectionGroupData SelectionGroup => _selectionGroupData;

        public void Awake()
        {
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetData(Reference, out _selectionGroupData) ?? false)
            {
                SelectionGroup.IsDynamic = true;
                SelectionGroup.Reverse = Reverse;
                SelectionGroup.Selections = Selections;
            }
        }
    }
}
