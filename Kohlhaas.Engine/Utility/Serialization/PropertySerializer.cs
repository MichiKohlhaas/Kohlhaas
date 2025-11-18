using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Serialization;

public class PropertySerializer : IRecordSerializer<PropertyRecord>
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
    private readonly PropertyBlockSerializer _blockSerializer = new();
    
    public PropertyRecord Deserialize(byte[] bytes) => Deserialize(bytes.AsSpan());

    public PropertyRecord Deserialize(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != RecordSize) throw new Exception($"Expected {RecordSize} bytes, got {bytes.Length}");
        
        var blocks = new PropertyBlock[NumBlocks];
        for (int i = Block1Pos, j = 0; i < RecordSize && j < NumBlocks; i += sizeof(ulong), j++)
        {
            blocks[j] = _blockSerializer.Deserialize(bytes.Slice(i, sizeof(ulong)));
        }
        
        return new PropertyRecord(
            inUse: bytes[InUsePos],
            nextPropId: BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(NextPropId, sizeof(short))),
            prevPropId: BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(PrevPropId, sizeof(short))),
            propertyBlocks: blocks);
    }

    public byte[] Serialize(PropertyRecord record)
    {
        var data = new byte[RecordSize];
        data[InUsePos] = record.InUse;
        BitConverter.GetBytes(record.NextPropId).CopyTo(data, NextPropId);
        BitConverter.GetBytes(record.PrevPropId).CopyTo(data, PrevPropId);
        for (int i = 0; i < record.PropertyBlocks.Length; i++)
        {
            var serializedPropertyBlock = _blockSerializer.Serialize(record.PropertyBlocks[i]);
            var copyIndex = Block1Pos + i * sizeof(ulong);
            serializedPropertyBlock.CopyTo(data, copyIndex);
        }
        return data;
    }
}

public class PropertyBlockSerializer : IRecordSerializer<PropertyBlock>
{
    private const byte BlockSize = 8;
    private const byte NibbleSize = 4;
    private const byte NibbleShiftAndMask = 0x0F;
    private const byte KeyPos = 0;
    private const byte ByteShiftAndMask = 0xFF;
    
    public PropertyBlock Deserialize(byte[] bytes) => Deserialize(bytes.AsSpan());

    //Test because I'm not sure of the endianness...
    public PropertyBlock Deserialize(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != BlockSize) throw new Exception($"Expected {BlockSize} bytes, got {bytes.Length}");
        //var blockData = BitConverter.ToUInt64(bytes);
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

    public byte[] Serialize(PropertyBlock record)
    {
        var data = new byte[BlockSize];
        /*
         * Key = 0x0A
         * 0x0A << 4 → 0xA0
         * PropertyType = 0x00BCDEF1
         * 0x00BCDEF1 >> 20 → 0x0000000B
         * 0xA0 | 0x0B = 0xAB
         *
         * 0x00BCDEF1 >> 12 → 0x00000BCD
         * 0x00000BCD & 0xFF → 0xCD
         */
        data[0] = (byte)((byte)(record.Key << NibbleSize) | (record.PropertyType >> 20));
        data[1] = (byte)((record.PropertyType >> 12) & ByteShiftAndMask);
        data[2] = (byte)((record.PropertyType >> 4) & ByteShiftAndMask);
        /*
         * 0x00BCDEF1 << 32 → 0xF1
         * 0xF1 & 0x0F → 0x01
         * 0x01 << 4 → 0x10
         *
         * Value = 0x23456789A (ulong, 36 bits)
         * 0x23456789A >> 32 → 0x02
         * 0x10 | 0x02 → 0x12
         */
        var lowerPropTypeBit = (byte)(GetLowerNibble((byte)(record.PropertyType << 32)) << NibbleSize);
        var upperValueBit = GetUpperNibble((byte)(record.Value >> 32));
        data[3] = (byte)(lowerPropTypeBit | upperValueBit);

        data[4] = (byte)((record.Value >> 24) & ByteShiftAndMask);
        data[5] = (byte)((record.Value >> 16) & ByteShiftAndMask);
        data[6] = (byte)((record.Value >> 8) & ByteShiftAndMask);
        data[7] = (byte)((record.Value >> 0) & ByteShiftAndMask);
        return data;
    }
    
    private byte GetUpperNibble(byte value)
    {
        return (byte)(value >> NibbleSize);
    }

    private byte GetLowerNibble(byte value)
    {
        return (byte)(value & NibbleShiftAndMask);
    }
}

public class PropertyIndexSerializer : IRecordSerializer<PropertyIndexRecord>
{
    private const byte RecordSize = 16;
    private const byte InUsePos = 0;
    private const byte RoNPos = 1;
    private const byte IdPos = 2;
    private const byte RecordLengthPos = 6;
    private const byte KeyPos = 7;
    private const byte NamePos = 8;
    
    public PropertyIndexRecord Deserialize(byte[] bytes) => Deserialize(bytes.AsSpan());

    public PropertyIndexRecord Deserialize(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != RecordSize) throw new Exception($"Expected {RecordSize} bytes, got {bytes.Length}");
        return new PropertyIndexRecord()
        {
            InUse = bytes[InUsePos],
            RoN = bytes[RoNPos],
            Id = BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(IdPos, sizeof(int))),
            RecordLength = bytes[RecordLengthPos],
            Key = bytes[KeyPos],
            Name = BinaryPrimitives.ReadUInt16LittleEndian(bytes.Slice(NamePos, sizeof(ulong))),
        };
    }

    public byte[] Serialize(PropertyIndexRecord record)
    {
        var data = new byte[RecordSize];
        data[InUsePos] = record.InUse;
        data[RoNPos] = record.RoN;
        BitConverter.GetBytes(record.Id).CopyTo(data, IdPos);
        data[RecordLengthPos] = record.RecordLength;
        data[KeyPos] = record.Key;
        BitConverter.GetBytes(record.Name).CopyTo(data, NamePos);
        return data;
    }
}