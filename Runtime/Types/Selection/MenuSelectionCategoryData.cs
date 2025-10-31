using UnityEngine;

namespace UnityEssentials
{
    public class MenuSelectionCategoryData : MenuTypeDataBase
    {
        public ScriptableObject[] Data;

        public int Default;

        public MenuSelectionDataElement GetSelection(int index)
        {
            foreach (var scriptableObject in Data)
                if (scriptableObject is MenuSelectionGroupData group)
                {
                    var selections = group.GetSelections();
                    if (selections == null || selections.Data == null)
                        continue;

                    for (int i = 0; i < selections.Data.Length; i++)
                        if (selections.StartIndexID + i == index)
                            return selections.Data[i];
                }

            return null;
        }

        public override object GetDefault() => Default;

        public override void ApplyDynamicReset() =>
            Default = 0;
    }
}