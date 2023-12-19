// JsonUnixDateTime attribute
// Converts this ya know date(105810823123) thing to a usable 
// date 

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zapp.Attributes;

public class UnixDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Value must be of type string");
        }

        string dateString = reader.GetString();
        long ticks = long.Parse(dateString.Substring(6, dateString.Length - 8));
        return new DateTime(
            ticks * 10_000 + new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks
        );
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

public class JsonUnixDateTimeAttribute : JsonConverterAttribute {
    public JsonUnixDateTimeAttribute() : base(typeof(UnixDateTimeConverter)) {}
}
