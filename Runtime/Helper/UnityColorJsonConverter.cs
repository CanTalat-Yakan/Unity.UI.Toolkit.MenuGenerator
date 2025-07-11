using System;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public class UnityColorJsonConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            string colorArray = $"[{value.r}, {value.g}, {value.b}, {value.a}]";
            writer.WriteRawValue(colorArray);
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float r = 0, g = 0, b = 0, a = 1;
            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                r = Convert.ToSingle(reader.Value);
                reader.Read();
                g = Convert.ToSingle(reader.Value);
                reader.Read();
                b = Convert.ToSingle(reader.Value);
                reader.Read();
                a = Convert.ToSingle(reader.Value);
                reader.Read();
            }
            return new Color(r, g, b, a);
        }
    }
}
