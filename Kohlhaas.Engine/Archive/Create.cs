using System.Collections.Immutable;
using Kohlhaas.Engine.Models;
using System.Linq;

namespace Kohlhaas.Engine;

internal static class Create
{
    internal static Node CreateNode(string vLevel, ImmutableDictionary<string, object> data, string tag, string fileName, string uniqueFileName)
    {
        var guid = Guid.CreateVersion7();
        return new Node
        {
            Id = guid, 
            VLevel = vLevel, 
            Properties = data, 
            FileTag = tag, 
            FileName = fileName, 
            UniqueFileName = uniqueFileName,
        };
    }

    internal static Relationship CreateRelationship(INode startNode, INode endNode, RelationshipType type, IDictionary<string, object> data, string label)
    {
        var guid = Guid.CreateVersion7();
        return new Relationship()
        {
            Id = guid,
            StartNode = startNode,
            EndNode = endNode,
            Label = label,
            Properties = data.ToImmutableDictionary(x => x.Key, x => x.Value),
            Type = type,
        };
    }

    internal static Graph? CreateGraph(INode[] nodes, IRelationship[] relationships)
    {
        if (nodes.Length != relationships.Length) return null;
        
        return new Graph(
            nodes.ToDictionary(n => n.Id),
            relationships.ToDictionary(r => r.Id)
        );
    }
}