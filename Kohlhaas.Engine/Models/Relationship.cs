using System.Collections;
using System.Collections.Immutable;

namespace Kohlhaas.Engine.Models;

public record Relationship : IRelationship
{
    public required Guid Id { get; init; }
    public required INode StartNode { get; init; }
    public required INode EndNode { get; init; }
    public required RelationshipType Type { get; init; }
    public required ImmutableDictionary<string, object> Properties { get; init; }
    public required string Label { get; init; }
    public IRelationship AddProperty(string name, object value)
    {
        return this with { Properties = Properties.Add(name, value) };
    }

    public IRelationship UpdateSelf(IRelationship update)
    {
        return new Relationship
        {
            Id = this.Id,
            StartNode = update.StartNode,
            EndNode = update.EndNode,
            Type = update.Type,
            Properties = update.Properties.ToImmutableDictionary(),
            Label = update.Label,
        };
    }
}