using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kohlhaas.Engine.Models;

namespace Kohlhaas.Engine.Converters;

public class NodeConverter : JsonConverter<INode> 
{
    public override INode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;
        
        var id = root.GetProperty("ID").GetGuid();
        var vLvl = root.GetProperty("VLevel").GetString();
        //DeserializeProperties(root.GetProperty("Properties"));
        var props = new Dictionary<string, object>().ToImmutableDictionary();
        var tag = root.GetProperty("FileTag").GetString();
        var fileName = root.GetProperty("FileName").GetString();
        var uFilename = root.GetProperty("UniqueFileName").GetString();
        
        
        return new Node
        {
            Id = id, 
            VLevel = vLvl, 
            Properties = props, 
            FileTag = tag, 
            FileName = fileName, 
            UniqueFileName = uFilename, 
        };
    }

    public override void Write(Utf8JsonWriter writer, INode value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("ID", value.Id);
        writer.WriteString("VLevel", value.VLevel);
        writer.WritePropertyName("Properties");
        JsonSerializer.Serialize(writer, value.Properties, options);
        writer.WriteString("FileTag", value.FileTag);
        writer.WriteString("FileName", value.FileName);
        writer.WriteString("UniqueFileName", value.UniqueFileName);
        
        
        writer.WriteEndObject();
    }
    
    /// <summary>
    /// Not used (yet).
    /// </summary>
    /// <param name="propertiesElement"></param>
    /// <returns></returns>
    private ImmutableDictionary<string, object> DeserializeProperties(JsonElement propertiesElement)
    {
        var builder = ImmutableDictionary.CreateBuilder<string, object>();
        
        foreach (var property in propertiesElement.EnumerateObject())
        {
            object value = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString()!,
                JsonValueKind.Number => property.Value.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                _ => property.Value.GetRawText()
            };
            
            builder.Add(property.Name, value);
        }
        
        return builder.ToImmutable();
    }
}