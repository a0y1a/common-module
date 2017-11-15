using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using KeKeSoftPlatform.Common;

namespace KeKeSoftPlatform.Core
{
    public class EnumConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //try
            //{
            //    var value = serializer.Deserialize<string>(reader);
            //    return Enum.Parse(objectType, value);
            //}
            //catch
            //{
            //}

            return Enum.Parse(objectType, serializer.Deserialize(reader).ToString());
        }
        
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            var type = value.GetType();
            var fieldName = type.GetEnumName(value);
            if (fieldName != null)
            {
                var localozation = type.GetField(fieldName).GetCustomAttributes(typeof(EnumValueAttribute), false).First() as EnumValueAttribute;
                serializer.Serialize(writer, localozation.Value);
            }
            else
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
