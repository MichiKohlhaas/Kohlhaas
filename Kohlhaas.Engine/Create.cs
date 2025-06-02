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

    internal static Relationship CreateRelationship(INode startNode, INode endNode, RelationshipType type, IDictionary<string, object> data)
    {
        var guid = Guid.CreateVersion7();
        return new Relationship(guid, startNode, endNode, type, data);
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