using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuColorPickerDataConfigurator : MonoBehaviour
    {
        public UIMenuData MenuData;
        public string Reference;

        [Space]
        public Color Default;

        private UIMenuColorPickerData _colorPickerData;
        [HideInInspector] public UIMenuColorPickerData ColorPickerData => _colorPickerData;

        public void Awake()
        {
            if (MenuData?.GetDataByReference(Reference, out _colorPickerData) ?? false)
            {
                ColorPickerData.IsDynamic = true;
                ColorPickerData.Default = Default;
            }
        }
    }
}
