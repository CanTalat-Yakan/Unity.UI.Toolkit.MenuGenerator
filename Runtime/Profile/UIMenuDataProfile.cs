using UnityEngine;

namespace UnityEssentials
{
    public abstract class UIMenuDataProfile : ScriptableObject
    {
        public SerializedDictionary<string, bool> ToggleDataDictionary;
        public SerializedDictionary<string, string> InputDataDictionary;
        public SerializedDictionary<string, int> OptionsDataDictionary;
        public SerializedDictionary<string, float> SliderDataDictionary;
        public SerializedDictionary<string, int> SwapperDataDictionary;
        public SerializedDictionary<string, int> SelectionDataDictionary;
        public SerializedDictionary<string, Color> ColorPickerDataDictionary;
        public SerializedDictionary<ColorSliderData, int> ColorSliderDataDictionary;

        public void CopyValues<T>(T source) where T : UIMenuDataProfile
        {
            ToggleDataDictionary.CopyFrom(source.ToggleDataDictionary?.Dictionary);
            InputDataDictionary.CopyFrom(source.InputDataDictionary?.Dictionary);
            OptionsDataDictionary.CopyFrom(source.OptionsDataDictionary?.Dictionary);
            SliderDataDictionary.CopyFrom(source.SliderDataDictionary?.Dictionary);
            SwapperDataDictionary.CopyFrom(source.SwapperDataDictionary?.Dictionary);
            SelectionDataDictionary.CopyFrom(source.SelectionDataDictionary?.Dictionary);
            ColorPickerDataDictionary.CopyFrom(source.ColorPickerDataDictionary?.Dictionary);
            ColorSliderDataDictionary.CopyFrom(source.ColorSliderDataDictionary?.Dictionary);
        }

        public virtual void OnToggleChange(string reference, bool value) =>
            ToggleDataDictionary[reference] = value;

        public virtual void OnInputChange(string reference, string input) =>
            InputDataDictionary[reference] = input;

        public virtual void OnOptionsChange(string reference, int index) =>
            OptionsDataDictionary[reference] = index;

        public virtual void OnSliderChange(string reference, float value) =>
            SliderDataDictionary[reference] = value;

        public virtual void OnSwapperChange(string reference, int index) =>
            SwapperDataDictionary[reference] = index;

        public virtual void OnSelectionChange(string reference, int index) =>
            SelectionDataDictionary[reference] = index;

        public virtual void OnColorPickerChange(string reference, Color color) =>
            ColorPickerDataDictionary[reference] = color;

        public virtual void OnColorSliderChange(ColorSliderData colorSliderData, int value) =>
            ColorSliderDataDictionary[colorSliderData] = value;
    }
}