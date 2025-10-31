using System;
using UnityEngine;

namespace UnityEssentials
{
    public class MenuProfile : ScriptableObject
    {
        [HideInInspector] public SerializedDictionary<string, object> Data = new();

        public Action<string> OnValueChanged;

        public void Set<T>(string reference, T value) =>
            Add(reference, value);

        public void Add(string reference, object value)
        {
            if (value == null)
                return;
            Data[reference] = value;
            OnValueChanged?.Invoke(reference);
        }

        public T Get<T>(MenuTypeDataBase type) =>
            Get(type.Reference, (T)type.GetDefault());

        public T Get<T>(string reference, T defaultValue = default)
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

        public static bool TryGetProfile(string name, out MenuProfile profile) =>
            UIMenuProfileProvider.TryGetProfile(name, out profile);
    }
}