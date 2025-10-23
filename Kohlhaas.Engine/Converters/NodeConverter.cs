using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kohlhaas.Engine.Models;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Converters;

public class NodeConverter 
{
    public static NodeRecord Convert(INode node)
    {
        return new NodeRecord(1, 1, 1, 1);
    }
}