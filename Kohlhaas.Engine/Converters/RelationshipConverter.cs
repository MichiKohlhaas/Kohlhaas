using System.Text.Json;
using System.Text.Json.Serialization;
using Kohlhaas.Common.Interfaces;
using Kohlhaas.Common.Models;

namespace Kohlhaas.Engine.Converters;

public class RelationshipConverter : JsonConverter<IRelationship>
{
    public override IRelationship? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return JsonSerializer.Deserialize<Relationship>(ref reader, options);
    }

    public override void Write(Utf8JsonWriter writer, IRelationship value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}