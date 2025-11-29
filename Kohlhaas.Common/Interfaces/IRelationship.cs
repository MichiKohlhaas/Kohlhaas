using System.Collections.Immutable;
using Kohlhaas.Common.Enums;

namespace Kohlhaas.Common.Interfaces;

public interface IRelationship
{
    public int Id { get; }
    public INode StartNode { get; }
    public INode EndNode { get; }
    public RelationshipType Type { get; }
    public ImmutableDictionary<string, object> Properties { get; }
    public string Label { get; }

    public abstract IRelationship AddProperty(string name, object value);

    public abstract IRelationship UpdateSelf(IRelationship update);
}