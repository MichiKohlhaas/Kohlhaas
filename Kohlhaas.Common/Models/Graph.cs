using Kohlhaas.Common.Interfaces;
namespace Kohlhaas.Common.Models;

public record Graph(IDictionary<Guid, INode> Nodes, IDictionary<Guid, IRelationship> Relationships) : IGraph;