using System;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public class UIMenuDataProfile : ScriptableObject
    {
        public SerializedDictionary<string, bool> Toggles = new();
        public SerializedDictionary<string, string> Inputs = new();
        public SerializedDictionary<string, int> Options = new();
        public SerializedDictionary<string, float> Sliders = new();
        public SerializedDictionary<string, int> Selections = new();
        public SerializedDictionary<string, Color> ColorPickers = new();
        public SerializedDictionary<string, float> ColorSliders = new();

        [JsonIgnore]
        public Action OnValueChanged;

        public void CopyFrom<T>(T source) where T : UIMenuDataProfile
        {
            Toggles.CopyFrom(source.Toggles?.Dictionary);
            Inputs.CopyFrom(source.Inputs?.Dictionary);
            Options.CopyFrom(source.Options?.Dictionary);
            Sliders.CopyFrom(source.Sliders?.Dictionary);
            Selections.CopyFrom(source.Selections?.Dictionary);
            ColorPickers.CopyFrom(source.ColorPickers?.Dictionary);
            ColorSliders.CopyFrom(source.ColorSliders?.Dictionary);
        }

        public void AddFrom<T>(T source) where T : UIMenuDataProfile
        {
            Toggles.AddFrom(source.Toggles?.Dictionary);
            Inputs.AddFrom(source.Inputs?.Dictionary);
            Options.AddFrom(source.Options?.Dictionary);
            Sliders.AddFrom(source.Sliders?.Dictionary);
            Selections.AddFrom(source.Selections?.Dictionary);
            ColorPickers.AddFrom(source.ColorPickers?.Dictionary);
            ColorSliders.AddFrom(source.ColorSliders?.Dictionary);
        }

        public virtual void OnToggleValueChanged(string reference, bool value)
        {
            Toggles[reference] = value;
            OnValueChanged?.Invoke();
        }

        public virtual void OnInputValueChanged(string reference, string input)
        {
            Inputs[reference] = input;
            OnValueChanged?.Invoke();
        }

        public virtual void OnOptionsValueChanged(string reference, int index)
        {
            Options[reference] = index;
            OnValueChanged?.Invoke();
        }

        public virtual void OnSliderValueChanged(string reference, float value)
        {
            Sliders[reference] = value;
            OnValueChanged?.Invoke();
        }

        public virtual void OnSelectionValueChanged(string reference, int index)
        {
            Selections[reference] = index;
            OnValueChanged?.Invoke();
        }

        public virtual void OnColorPickerValueChanged(string reference, Color color)
        {
            ColorPickers[reference] = color;
            OnValueChanged?.Invoke();
        }

        public virtual void OnColorSliderValueChanged(string reference, int value)
        {
            ColorSliders[reference] = value;
            OnValueChanged?.Invoke();
        }
    }
}