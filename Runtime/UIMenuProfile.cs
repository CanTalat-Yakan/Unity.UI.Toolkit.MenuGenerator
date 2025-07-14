using Newtonsoft.Json;
using System;
using UnityEngine;

namespace UnityEssentials
{
    public class UIMenuProfile : ScriptableObject
    {
        [HideInInspector] public SerializedDictionary<string, object> Data = new();

        public Action<string> OnValueChanged;
        public void SetData<T>(string reference, T value) =>
            AddData(reference, value);

        public void AddData(string reference, object value)
        {
            if (value == null)
                return;
            Data[reference] = value;
            OnValueChanged?.Invoke(reference);
        }

        public T GetData<T>(string reference, T defaultValue = default)
        {
            if (Data.TryGetValue(reference, out var value))
            {
                if (value is T typedValue)
                    return typedValue;
                if (typeof(T) == typeof(Color))
                    return UnityColorJsonConverter.DeserializeColor(value, defaultValue);
                if (value is IConvertible convertibleValue)
                    return (T)Convert.ChangeType(convertibleValue, typeof(T));
                if (value is Enum enumValue && typeof(T).IsEnum)
                    return (T)(object)enumValue;
            }
            return defaultValue;
        }
    }
}