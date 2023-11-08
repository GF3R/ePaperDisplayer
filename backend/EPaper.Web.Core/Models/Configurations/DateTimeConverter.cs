using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EPaper.Web.Core.Models.Configurations
{
    public class DateTimeConverter : JsonConverter<DateTimeOffset?>
    {
        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringDate = reader.GetString();
            return string.IsNullOrWhiteSpace(stringDate) ? (DateTimeOffset?)null : DateTimeOffset.Parse(stringDate);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}