using System;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public class UIMenuDataProfile : ScriptableObject
    {
        public SerializedDictionary<string, bool> ToggleDataDictionary = new();
        public SerializedDictionary<string, string> InputDataDictionary = new();
        public SerializedDictionary<string, int> OptionsDataDictionary = new();
        public SerializedDictionary<string, float> SliderDataDictionary = new();
        public SerializedDictionary<string, int> SelectionDataDictionary = new();
        public SerializedDictionary<string, Color> ColorPickerDataDictionary = new();
        public SerializedDictionary<string, int> ColorSliderDataDictionary = new();

        [JsonIgnore]
        public Action OnValueChanged;

        public void CopyValues<T>(T source) where T : UIMenuDataProfile
        {
            ToggleDataDictionary.CopyFrom(source.ToggleDataDictionary?.Dictionary);
            InputDataDictionary.CopyFrom(source.InputDataDictionary?.Dictionary);
            OptionsDataDictionary.CopyFrom(source.OptionsDataDictionary?.Dictionary);
            SliderDataDictionary.CopyFrom(source.SliderDataDictionary?.Dictionary);
            SelectionDataDictionary.CopyFrom(source.SelectionDataDictionary?.Dictionary);
            ColorPickerDataDictionary.CopyFrom(source.ColorPickerDataDictionary?.Dictionary);
            ColorSliderDataDictionary.CopyFrom(source.ColorSliderDataDictionary?.Dictionary);
        }

        public virtual void OnToggleValueChanged(string reference, bool value)
        {
            ToggleDataDictionary[reference] = value;
            OnValueChanged?.Invoke();
        }

        public virtual void OnInputValueChanged(string reference, string input)
        {
            InputDataDictionary[reference] = input;
            OnValueChanged?.Invoke();
        }

        public virtual void OnOptionsValueChanged(string reference, int index)
        {
            OptionsDataDictionary[reference] = index;
            OnValueChanged?.Invoke();
        }

        public virtual void OnSliderValueChanged(string reference, float value)
        {
            SliderDataDictionary[reference] = value;
            OnValueChanged?.Invoke();
        }

        public virtual void OnSelectionValueChanged(string reference, int index)
        {
            SelectionDataDictionary[reference] = index;
            OnValueChanged?.Invoke();
        }

        public virtual void OnColorPickerValueChanged(string reference, Color color)
        {
            ColorPickerDataDictionary[reference] = color;
            OnValueChanged?.Invoke();
        }

        public virtual void OnColorSliderValueChanged(string reference, int value)
        {
            ColorSliderDataDictionary[reference] = value;
            OnValueChanged?.Invoke();
        }
    }
}