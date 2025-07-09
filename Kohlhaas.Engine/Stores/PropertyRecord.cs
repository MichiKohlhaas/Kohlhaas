namespace Kohlhaas.Engine.Stores;

/// <summary>
/// Represents an 8-byte block of property data.
/// 4 bits: key, pointer to the kohlhaas.propertystore.db.index where property name is stored. <br/>
/// 24 bits: property type, primitive, string, or array of primitives. <br/>
/// We assume that the first byte's significant bits correspond to the key
/// Remaining 4.5 bytes: value of the block
/// </summary>
public readonly struct PropertyBlock(byte key, uint propType, ulong value)
{
    public byte Key { get; init; } = key;
    public uint PropertyType { get; init; } = propType;
    public ulong Value { get; init; } =  value;
}

/// <summary>
/// <para>
/// Each property record has 4 property blocks (8 bytes ea.) and two pointers
/// to the next and previous property in the list, respectively.
/// A property can occupy b/w 1 - 4 property blocks. <see cref="PropertyBlock"/>
/// </para>
/// Byte 1 = in use <br/>
/// Byte 2 - 3  = next property <br/>
/// Byte 4 - 5 = previous property, 0 if first <br/>
/// Byte 6 - 13 = Property block 1 <br/>
/// Byte 14 - 21 = Property block 2 <br/>
/// Byte 22 - 29 = Property block 3 <br/>
/// Byte 30 - 37 = Property block 4
/// </summary>
public readonly struct PropertyRecord(byte inUse, ushort nextPropId, ushort prevPropId, PropertyBlock[] propertyBlocks)
{
    public byte InUse { get; init; } = inUse;
    public ushort NextPropId { get; init; } = nextPropId;
    public ushort PrevPropId { get; init; } = prevPropId;
    public PropertyBlock[] PropertyBlocks { get; init; } = propertyBlocks;
    /*public PropertyBlock Block1 { get; init; } = propertyBlocks[0];
    public PropertyBlock Block2 { get; init; } = propertyBlocks[1];
    public PropertyBlock Block3 { get; init; } = propertyBlocks[2];
    public PropertyBlock Block4 { get; init; } = propertyBlocks[3];*/
}