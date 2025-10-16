namespace Kohlhaas.Engine.Stores;

/// <summary>
/// Byte 1: inUse
/// Byte 2 - 4: reserved
/// Byte 5 - 24: [:Label1]
/// Byte 25 - 44: [:Label2]
/// Byte 45 - 64: [:Label3]
/// Nodes/Relationships can have 3 labels, set aside in blocks of 20 bytes each.
/// </summary>
public readonly struct LabelRecord(byte inUse, uint reservedSpace, byte[] labelData)
{
    private const byte BlockSize = 20;

    public byte InUse { get; init; } = inUse;
    public uint ReservedSpace { get; init; } = reservedSpace;
    public byte[] LabelData { get; init; } = labelData;
}