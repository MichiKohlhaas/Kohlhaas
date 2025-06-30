using System.Diagnostics.CodeAnalysis;

namespace Kohlhaas.Engine.Stores;

public readonly struct NodeRecord(byte inUse, uint labels, uint nextRelId, uint nextPropId)
{
    public byte InUse { get; init; } = inUse;
    public uint Labels { get; init; } = labels;
    public uint NextRelId { get; init; } = nextRelId;
    public uint NextPropId { get; init; } = nextPropId;
}