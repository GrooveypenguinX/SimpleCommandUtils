using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SimpleCommandUtils.Utils;

public class CustomJsonConverter : JsonConverter<Vector3>
{
    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartObject)
        {
            JObject obj = JObject.Load(reader);
            float x = obj["x"]?.Value<float>() ?? 0;
            float y = obj["y"]?.Value<float>() ?? 0;
            float z = obj["z"]?.Value<float>() ?? 0;
            return new Vector3(x, y, z);
        }

        if (reader.TokenType == JsonToken.StartArray)
        {
            JArray array = JArray.Load(reader);
            if (array.Count < 3)
                throw new FormatException("Array does not contain enough elements");
            float x = array[0].Value<float>();
            float y = array[1].Value<float>();
            float z = array[2].Value<float>();
            return new Vector3(x, y, z);
        }
        throw new FormatException("Unexpected token when parsing Vector3.");
    }

    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(value.x);
        writer.WritePropertyName("y");
        writer.WriteValue(value.y);
        writer.WritePropertyName("z");
        writer.WriteValue(value.z);
        writer.WriteEndObject();
    }
}