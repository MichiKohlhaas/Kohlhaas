using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Serialization;

public class RelationshipRecordSerializer : IRecordSerializer<RelationshipRecord>
{
    private const byte RecordSize = 30;
    private const byte InUsePos = 0;
    private const byte FirstNodePos = 1;
    private const byte SecondNodePos = 5;
    private const byte RelTypePos = 9;
    private const byte FirstPrevRelateIdPos = 10;
    private const byte FirstNextRelateIdPos = 14;
    private const byte SecondPrevRelateIdPos = 18;
    private const byte SecondNextRelateIdPos = 22;
    private const byte NextPropertyIdPos = 26;
    
    public RelationshipRecord Deserialize(byte[] bytes) => Deserialize(bytes.AsSpan());

    public RelationshipRecord Deserialize(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != RecordSize) throw new Exception($"Expected {RecordSize} bytes, got {bytes.Length}");

        return new RelationshipRecord(
            inUse: bytes[InUsePos],
            firstNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(FirstNodePos,sizeof(uint))),
            secondNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(SecondNodePos,sizeof(uint))),
            relationshipType: bytes[RelTypePos],
            firstPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(FirstPrevRelateIdPos,sizeof(uint))),
            firstNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(FirstNextRelateIdPos,sizeof(uint))),
            secondPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(SecondPrevRelateIdPos, sizeof(uint))),
            secondNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(SecondNextRelateIdPos, sizeof(uint))),
            nextPropId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(NextPropertyIdPos, sizeof(uint)))
        );
    }

    public byte[] Serialize(RelationshipRecord record)
    {
        var data = new byte[RecordSize];
        data[InUsePos] = record.InUse;
        BitConverter.GetBytes(record.FirstNode).CopyTo(data, FirstNodePos);
        BitConverter.GetBytes(record.SecondNode).CopyTo(data, SecondNodePos);
        data[RelTypePos] = record.RelationshipType;
        BitConverter.GetBytes(record.FirstPrevRelId).CopyTo(data, FirstPrevRelateIdPos);
        BitConverter.GetBytes(record.FirstNextRelId).CopyTo(data, FirstNextRelateIdPos);
        BitConverter.GetBytes(record.SecondPrevRelId).CopyTo(data, SecondPrevRelateIdPos);
        BitConverter.GetBytes(record.SecondNextRelId).CopyTo(data, SecondNextRelateIdPos);
        BitConverter.GetBytes(record.NextPropId).CopyTo(data, NextPropertyIdPos);
        return data;
    }
}