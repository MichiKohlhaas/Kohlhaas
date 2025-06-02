using System.Collections;

namespace Kohlhaas.Engine.Models;

public record Relationship(Guid Id, INode StartNode, INode EndNode, RelationshipType Type, IDictionary<string, object> Properties) : IRelationship
{
    public Guid Id { get; set; } = Id;
    public INode StartNode { get; set; } = StartNode;
    public INode EndNode { get; set; } = EndNode;
    public RelationshipType Type { get; set; } = Type;
    public IDictionary<string, object> Properties { get; set; } = Properties;
}