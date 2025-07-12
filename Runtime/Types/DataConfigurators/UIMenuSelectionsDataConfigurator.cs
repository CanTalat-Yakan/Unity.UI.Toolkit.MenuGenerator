using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionsDataConfigurator : MonoBehaviour
    {
        public string MenuName;
        public string Reference;

        [Space]
        public bool Reverse;
        public UIMenuSelectionData[] Selections;

        private UIMenuSelectionGroupData _selectionGroup;
        [HideInInspector] public UIMenuSelectionGroupData SelectionGroup => _selectionGroup;

        public void Awake()
        {
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetDataByReference(Reference, out _selectionGroup) ?? false)
            {
                SelectionGroup.IsDynamic = true;
                SelectionGroup.Reverse = Reverse;
                SelectionGroup.Selections = Selections;
            }
        }
    }
}
