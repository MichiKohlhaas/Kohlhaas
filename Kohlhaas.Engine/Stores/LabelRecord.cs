namespace Kohlhaas.Engine.Stores;

/// <summary>
/// Byte 1: inUse
/// Byte 2 - 5: reserved
/// Byte 6 - 25: [:Label1]
/// Byte 26 - 45: [:Label2]
/// Byte 46 - 65: [:Label3]
/// Nodes/Relationships can have 3 labels, set aside in blocks of 20 bytes each.
/// </summary>
public readonly struct LabelRecord(byte inUse, uint reservedSpace, byte[] labelData)
{
    private const byte BlockSize = 20;

    public byte InUse { get; init; } = inUse;
    public uint ReservedSpace { get; init; } = reservedSpace;
    /// <summary>
    /// If one label block is 20 bytes, that means a string (Unicode) can have 10 characters (assuming regular 2 bytes each).
    /// </summary>
    public byte[] LabelData { get; init; } = labelData;
    
}