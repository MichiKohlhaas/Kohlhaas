namespace Kohlhaas.Engine.Stores;

public readonly struct NodeRecord(byte inUse, uint labels, uint nextRelId, uint nextPropId)
{
    public byte InUse { get; init; } = inUse;
    /// <summary>
    /// ID, pointer to location on disk
    /// </summary>
    public uint Labels { get; init; } = labels;
    public uint NextRelId { get; init; } = nextRelId;
    public uint NextPropId { get; init; } = nextPropId;
}