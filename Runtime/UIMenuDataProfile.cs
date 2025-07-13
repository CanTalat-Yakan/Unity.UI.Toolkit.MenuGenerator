using System;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public class UIMenuDataProfile : ScriptableObject
    {
        [HideInInspector] public SerializedDictionary<string, object> Data = new();

        [JsonIgnore]
        public Action OnValueChanged;

        public void AddData(string reference, object value)
        {
            if (value == null)
                return;
            Data[reference] = value;
            OnValueChanged?.Invoke();
        }

        public T GetData<T>(string reference, T defaultValue = default)
        {
            if (Data.TryGetValue(reference, out var value))
                if(value is T typedValue)
                    return typedValue;
            else if (value is IConvertible convertibleValue)
                return (T)Convert.ChangeType(convertibleValue, typeof(T));
            else if (value is Enum enumValue && typeof(T).IsEnum)
                return (T)(object)enumValue;
            return defaultValue;
        }

        [JsonIgnore]
        public Action<string, bool> OnToggleValueChangedEvent;
        public virtual void OnToggleValueChanged(string reference, bool value)
        {
            Data[reference] = value;
            OnToggleValueChangedEvent?.Invoke(reference, value);
            OnValueChanged?.Invoke();
        }

        [JsonIgnore]
        public Action<string, string> OnInputValueChangedEvent;
        public virtual void OnInputValueChanged(string reference, string input)
        {
            Data[reference] = input;
            OnInputValueChangedEvent?.Invoke(reference, input);
            OnValueChanged?.Invoke();
        }

        [JsonIgnore]
        public Action<string, int> OnOptionsValueChangedEvent;
        public virtual void OnOptionsValueChanged(string reference, int index)
        {
            Data[reference] = index;
            OnOptionsValueChangedEvent?.Invoke(reference, index);
            OnValueChanged?.Invoke();
        }

        [JsonIgnore]
        public Action<string, float> OnSliderValueChangedEvent;
        public virtual void OnSliderValueChanged(string reference, float value)
        {
            Data[reference] = value;
            OnSliderValueChangedEvent?.Invoke(reference, value);
            OnValueChanged?.Invoke();
        }

        [JsonIgnore]
        public Action<string, int> OnSelectionValueChangedEvent;
        public virtual void OnSelectionValueChanged(string reference, int index)
        {
            Data[reference] = index;
            OnSelectionValueChangedEvent?.Invoke(reference, index);
            OnValueChanged?.Invoke();
        }

        [JsonIgnore]
        public Action<string, Color> OnColorPickerValueChangedEvent;
        public virtual void OnColorPickerValueChanged(string reference, Color color)
        {
            Data[reference] = color;
            OnColorPickerValueChangedEvent?.Invoke(reference, color);
            OnValueChanged?.Invoke();
        }

        [JsonIgnore]
        public Action<string, int> OnColorSliderValueChangedEvent;
        public virtual void OnColorSliderValueChanged(string reference, int value)
        {
            Data[reference] = value;
            OnColorSliderValueChangedEvent?.Invoke(reference, value);
            OnValueChanged?.Invoke();
        }
    }
}