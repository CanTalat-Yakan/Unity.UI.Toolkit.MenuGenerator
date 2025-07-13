using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorPickerButtonDataConfigurator : MonoBehaviour
    {
        public string MenuName;
        public string Reference;

        [Space]
        public Color Default;

        private UIMenuColorPickerData _colorPickerData;
        [HideInInspector] public UIMenuColorPickerData ColorPickerData => _colorPickerData;

        public void Awake()
        {
            if (!UIMenu.Instances.TryGetValue(MenuName, out var menu))
                return;

            if (menu.Data?.GetDataByReference(Reference, out _colorPickerData) ?? false)
            {
                ColorPickerData.IsDynamic = true;
                ColorPickerData.Default = Default;
            }
        }
    }
}
