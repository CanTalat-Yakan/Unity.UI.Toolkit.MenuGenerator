using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuGeneratorDataProvider : MonoBehaviour
    {
        [Header("Data Configuration")]
        public UIMenuGenerator UIGenerator;

        [Space]
        public CategoryData RootCategory;

        public void PopulateUI()
        {
            if (!UIGenerator.ValidateDependencies())
            {
                Debug.LogError("Initialization failed: Essential dependencies are missing!");
                enabled = false;
                return;
            }

            UIGenerator.PopulateHierarchy(RootCategory.Name, false, RootCategory.Data);
        }

        public void ResetUI() =>
            UIGenerator.ResetUI();
    }
}