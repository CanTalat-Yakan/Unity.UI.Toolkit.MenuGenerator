using UnityEngine;

namespace UnityEssentials
{
    public class SelectionsDataConfigurator : MonoBehaviour
    {
        public UIMenuData MenuData;
        public string Reference;

        [Space]
        public bool Reverse;
        public UIMenuSelectionData[] Selections;

        private UIMenuSelectionDataGroup _selectionGroup;
        [HideInInspector] public UIMenuSelectionDataGroup SelectionGroup => _selectionGroup;

        public void Awake()
        {
            if (MenuData?.GetDataByReference(Reference, out _selectionGroup) ?? false)
            {
                SelectionGroup.IsDynamic = true;
                SelectionGroup.Reverse = Reverse;
                SelectionGroup.Selections = Selections;
            }
        }
    }
}
