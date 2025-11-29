/*using System.Collections.Immutable;

namespace Kohlhaas.Engine.Models;

public record Node : INode
{
    public int Id { get; init; }
    public string[]? Labels { get; init; }
    public List<IRelationship>? Relationships { get; init; }
    public ImmutableDictionary<string, object>? Properties { get; set; }

    public INode AddProperty(string key, object value)
    {
        Properties ??= ImmutableDictionary<string, object>.Empty;
        return this with { Properties = Properties.Add(key, value) };
    }

    public INode UpdateSelf(INode update)
    {
        return new Node
        {
            Id = update.Id, 
            Labels = update.Labels,
            Relationships = update.Relationships,
            Properties = update.Properties,
        };
    }
}*/