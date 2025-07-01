using System;
using UnityEngine;
using Newtonsoft.Json;

namespace UnityEssentials
{
    public class UnityColorJsonConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("r");
            writer.WriteValue(value.r);
            writer.WritePropertyName("g");
            writer.WriteValue(value.g);
            writer.WritePropertyName("b");
            writer.WriteValue(value.b);
            writer.WritePropertyName("a");
            writer.WriteValue(value.a);
            writer.WriteEndObject();
        }

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float r = 0, g = 0, b = 0, a = 1;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                    break;
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    string propName = (string)reader.Value;
                    reader.Read();
                    switch (propName)
                    {
                        case "r": r = Convert.ToSingle(reader.Value); break;
                        case "g": g = Convert.ToSingle(reader.Value); break;
                        case "b": b = Convert.ToSingle(reader.Value); break;
                        case "a": a = Convert.ToSingle(reader.Value); break;
                    }
                }
            }
            return new Color(r, g, b, a);
        }
    }
}
