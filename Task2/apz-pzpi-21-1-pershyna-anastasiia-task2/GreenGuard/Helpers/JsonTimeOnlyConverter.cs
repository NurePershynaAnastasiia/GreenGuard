using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var timeStr = reader.GetString();
        if (TimeOnly.TryParse(timeStr, out var time))
        {
            return time;
        }
        else
        {
            throw new JsonException($"Invalid time format: {timeStr}");
        }
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
