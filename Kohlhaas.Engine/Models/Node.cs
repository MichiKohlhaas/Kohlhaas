using System.Collections.Immutable;

namespace Kohlhaas.Engine.Models;

public record Node : INode
{
    public required Guid Id { get; init; }
    public required string VLevel { get; init; }
    public required ImmutableDictionary<string, object> Properties { get; init; }
    public required string FileTag { get; init; }
    public required string FileName { get; init; }
    public required string UniqueFileName { get; init; }

    public INode AddProperty(string name, object value)
    {
        return this with { Properties = Properties.Add(name, value) };
    }

    public INode UpdateSelf(INode update)
    {
        return new Node
        {
            Id = this.Id,
            VLevel = update.VLevel,
            Properties = update.Properties,
            FileTag = update.FileTag,
            FileName = update.FileName,
            UniqueFileName = update.UniqueFileName
        };
    }
}