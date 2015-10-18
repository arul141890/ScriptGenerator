// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RawJsonStringConverter.cs" company="Tapmobi">
//   Copyright (c) Tapmobi. All rights reserved.
// </copyright>
// <summary>
//   The raw json string converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CommonUtilitiesModule
{
    using System;
    using Newtonsoft.Json;

    public class RawJsonStringConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof (string);
        }

        public override object ReadJson(
            JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue((string) value);
        }

    }
}