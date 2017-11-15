using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeKeSoftPlatform.Db;

namespace KeKeSoftPlatform.Core
{
    public class UrlCollectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteValue("[]");
            }
            var collection = JsonConvert.DeserializeObject<string[]>(value.ToString());
            writer.WriteStartArray();
            for (int i = 0; i < collection.Length; i++)
            {
                collection[i] = Service.Url(collection[i]);
                writer.WriteValue(collection[i]);
            }
            writer.WriteEndArray();
        }
    }

    public class UrlConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            writer.WriteValue(Service.Url(value.ToString()));
        }
    }
}
