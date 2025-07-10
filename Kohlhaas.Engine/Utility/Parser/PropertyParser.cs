using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public class PropertyParser : IRecordParser<PropertyRecord>
{
    private const byte RecordSize = 37;
    private const byte NumBlocks = 4;
    private const byte InUsePos = 0;
    private const byte NextPropId = 1;
    private const byte PrevPropId = 3;
    private const byte Block1Pos = 5;
    /*private const byte Block2Pos = 13;
    private const byte Block3Pos = 21;
    private const byte Block4Pos = 29;*/
    private readonly PropertyBlockParser _blockParser = new PropertyBlockParser();
    
    public PropertyRecord ParseTo(byte[] bytes) => ParseTo(bytes.AsSpan());

    public PropertyRecord ParseTo(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != RecordSize) throw new Exception($"Expected {RecordSize} bytes, got {bytes.Length}");
        
        /*var block1 = _blockParser.ParseTo(bytes.Slice(Block1Pos, sizeof(ulong)));
        var block2 = _blockParser.ParseTo(bytes.Slice(Block2Pos,sizeof(ulong)));
        var block3 = _blockParser.ParseTo(bytes.Slice(Block3Pos, sizeof(ulong)));
        var block4 = _blockParser.ParseTo(bytes.Slice(Block4Pos, sizeof(ulong)));*/

        PropertyBlock[] blocks = new PropertyBlock[NumBlocks];
        for (int i = Block1Pos, j = 0; i < RecordSize && j < NumBlocks; i += sizeof(ulong), j++)
        {
            blocks[j] = _blockParser.ParseTo(bytes.Slice(i, sizeof(ulong)));
        }
        
        return new PropertyRecord(
            inUse: bytes[InUsePos],
            nextPropId: BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(NextPropId, sizeof(short))),
            prevPropId: BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(PrevPropId, sizeof(short))),
            propertyBlocks: blocks);
    }

    public byte[] ParseFrom(PropertyRecord record)
    {
        return [];
    }
}

public class PropertyBlockParser : IRecordParser<PropertyBlock>
{
    private const byte NibbleSize = 4;
    private const byte KeyPos = 0;
    
    
    public PropertyBlock ParseTo(byte[] bytes) => ParseTo(bytes.AsSpan());

    //Test because I'm not sure of the endianness...
    public PropertyBlock ParseTo(ReadOnlySpan<byte> bytes)
    {
        // grab the first 4 bits of byte 1
        var key = GetUpperNibble(bytes[KeyPos]);
        /*
         Grab the last 4 bits of byte 1 and create room for the remaining 20 bits of the 24-bit field
         Bytes 2 & 3 we grab normally, shifting them over into the available space,
         the last 4 bits are taken from first 4 bits of byte 4. 
        */
        var propType = (uint)((GetLowerNibble(bytes[KeyPos]) << 20) |
                              (bytes[1] << 12) |
                              (bytes[2] << 4) |
                              GetUpperNibble(bytes[3]));
        
        // same operation as getting the property type only we need a long for 36 bits
        var value = ((ulong)GetLowerNibble(bytes[3]) << 32) |
                      ((ulong)bytes[4] << 24) |
                      ((ulong)bytes[5] << 16) |
                      ((ulong)bytes[6] << 8) |
                      bytes[7];
        return new PropertyBlock(key, propType, value);
    }

    public byte[] ParseFrom(PropertyBlock record)
    {
        return [];
    }
    
    private byte GetUpperNibble(byte value)
    {
        return (byte)(value >> NibbleSize);
    }

    private byte GetLowerNibble(byte value)
    {
        return (byte)(value & 0x0F);
    }
}