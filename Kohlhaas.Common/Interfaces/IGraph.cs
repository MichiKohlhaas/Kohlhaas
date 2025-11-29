namespace Kohlhaas.Common.Interfaces;

public interface IGraph
{
    public IDictionary<Guid, INode> Nodes { get; }
    public IDictionary<Guid, IRelationship> Relationships { get; }
}