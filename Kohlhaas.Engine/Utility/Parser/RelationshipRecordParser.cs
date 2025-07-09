using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public class RelationshipRecordParser : IRecordParser<RelationshipRecord>
{
    private const byte RecordSize = 33;
    private const byte InUsePos = 0;
    private const byte FirstNodePos = 1;
    private const byte SecondNodePos = 5;
    private const byte RelTypePos = 9;
    private const byte FirstPrevRelateIdPos = 13;
    private const byte FirstNextRelateIdPos = 17;
    private const byte SecondPrevRelateIdPos = 21;
    private const byte SecondNextRelateIdPos = 25;
    private const byte NextPropertyIdPos = 29;
    
    public RelationshipRecord ParseTo(byte[] bytes) => ParseTo(bytes.AsSpan());

    public RelationshipRecord ParseTo(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != RecordSize) throw new Exception($"Expected {RecordSize} bytes, got {bytes.Length}");

        return new RelationshipRecord(
            inUse: bytes[InUsePos],
            firstNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(FirstNodePos,sizeof(uint))),
            secondNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(SecondNodePos,sizeof(uint))),
            relationshipType: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(RelTypePos,sizeof(uint))),
            firstPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(FirstPrevRelateIdPos,sizeof(uint))),
            firstNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(FirstNextRelateIdPos,sizeof(uint))),
            secondPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(SecondPrevRelateIdPos, sizeof(uint))),
            secondNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(SecondNextRelateIdPos, sizeof(uint))),
            nextPropId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(NextPropertyIdPos, sizeof(uint)))
        );
    }
}