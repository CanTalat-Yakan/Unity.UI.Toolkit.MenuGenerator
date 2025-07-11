using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuSelectionCategoryData : UIMenuGeneratorTypeTemplate
    {
        public ScriptableObject[] Data;

        public int Default;

        public UIMenuSelectionDataElement GetSelection(int index)
        {
            foreach (var scriptableObject in Data)
                if (scriptableObject is UIMenuSelectionGroupData group)
                    foreach (var selections in group.GetSelections())
                        if (selections != null && selections.Data != null)
                            for (int i = 0; i < selections.Data.Length; i++)
                                if (selections.StartIndexID + i == index)
                                    return selections.Data[i];

            return null;
        }

        public override object GetDefault() => Default;
    }
}
