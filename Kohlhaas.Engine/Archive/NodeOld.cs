/*using System.Collections.Immutable;

namespace Kohlhaas.Engine.Models;

public record NodeOld : INodeOld
{
    public required Guid Id { get; init; }
    public required string VLevel { get; init; }
    public required ImmutableDictionary<string, object> Properties { get; init; }
    public required string FileTag { get; init; }
    public required string FileName { get; init; }
    public required string UniqueFileName { get; init; }

    public INodeOld AddProperty(string name, object value)
    {
        return this with { Properties = Properties.Add(name, value) };
    }

    public INodeOld UpdateSelf(INodeOld update)
    {
        return new NodeOld
        {
            Id = this.Id,
            VLevel = update.VLevel,
            Properties = update.Properties,
            FileTag = update.FileTag,
            FileName = update.FileName,
            UniqueFileName = update.UniqueFileName
        };
    }
}*/