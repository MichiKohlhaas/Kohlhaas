using System.Buffers.Binary;

namespace Kohlhaas.Engine.Stores;

/// <summary>
/// inUse
///  |   | FirstNode   | | SecondNode  | | RelType     | |1stPrevRelId | |1stNextRelId | |2ndPrevRelId | |2ndNextRelId | | NextPropId  |
/// [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ]
///  1   2           5               9              13              17              21              25              29              33
/// </summary>
public readonly struct RelationshipRecord(
    byte inUse,
    uint firstNode,
    uint secondNode,
    uint relationshipType,
    uint firstPrevRelId,
    uint firstNextRelId,
    uint secondPrevRelId,
    uint secondNextRelId,
    uint nextPropId)
{
    public byte InUse { get; init; } = inUse;
    public uint FirstNode { get; init; } = firstNode;
    public uint SecondNode { get; init; } = secondNode;

    public uint RelationshipType { get; init; } = relationshipType;

    // prev = previous, rel = relationship
    public uint FirstPrevRelId { get; init; } = firstPrevRelId;
    public uint FirstNextRelId { get; init; } = firstNextRelId;
    public uint SecondPrevRelId { get; init; } = secondPrevRelId;
    public uint SecondNextRelId { get; init; } = secondNextRelId;
    public uint NextPropId { get; init; } = nextPropId;

    /*
    public RelationshipRecord(Span<byte> buffer)
    {
        if (buffer.Length != RecordSize) throw new ArgumentException($"Byte array is not {RecordSize} bytes in length)");
        
        InUse = buffer[InUsePos];
        FirstNode = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(FirstNodePos, sizeof(uint)));
        SecondNode = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(SecondNodePos, sizeof(uint)));
        RelationshipType = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(RelTypePos, sizeof(uint)));
        FirstPrevRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(FirstPrevRelateIdPos, sizeof(uint)));
        FirstNextRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(FirstNextRelateIdPos, sizeof(uint)));
        SecondPrevRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(SecondPrevRelateIdPos, sizeof(uint)));
        SecondNextRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(SecondNextRelateIdPos, sizeof(uint)));
        NextPropId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(NextPropertyIdPos, sizeof(uint)));
    }*/
}