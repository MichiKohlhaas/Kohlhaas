using System.Buffers.Binary;

namespace Kohlhaas.Engine.Stores;

/// <summary>
/// inUse
///  |   | FirstNode   | | SecondNode  | | RelType     | |1stPrevRelId | |1stNextRelId | |2ndPrevRelId | |2ndNextRelId | | NextPropId  |
/// [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ] [ ]
///  1   2           5               9              13              17              21              25              29              33               
/// 
/// </summary>
public readonly struct RelationshipRecord
{
    private const byte RecordSize = 33;
    public byte InUse { get; init; }
    public uint FirstNode { get; init; }
    public uint SecondNode { get; init; }
    public uint RelationshipType { get; init; }
    public uint FirstPrevRelId { get; init; }
    public uint FirstNextRelId { get; init; }
    public uint SecondPrevRelId { get; init; }
    public uint SecondNextRelId { get; init; }
    public uint NextPropId { get; init; }

    public RelationshipRecord(byte inUse, uint firstNode, uint secondNode, uint relationshipType,
        uint firstPrevRelId, uint firstNextRelId, uint secondPrevRelId, uint secondNextRelId, uint nextPropId)
    {
        InUse = inUse;
        FirstNode = firstNode;
        SecondNode = secondNode;
        RelationshipType = relationshipType;
        FirstPrevRelId = firstPrevRelId;
        FirstNextRelId = firstNextRelId;
        SecondPrevRelId = secondPrevRelId;
        SecondNextRelId = secondNextRelId;
        NextPropId = nextPropId;
    }
    
    public RelationshipRecord(byte[] buffer)
    {
        if (buffer.Length < 33) throw new ArgumentException($"Byte array is not {RecordSize}-bytes in length)");
        InUse = buffer[0];
        if (BitConverter.IsLittleEndian)
        {
            FirstNode = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[1..5]);
            SecondNode = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[5..9]);
            RelationshipType = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[9..13]);
            FirstPrevRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[13..17]);
            FirstNextRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[17..21]);
            SecondPrevRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[21..25]);
            SecondNextRelId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[25..29]);
            NextPropId = BinaryPrimitives.ReadUInt32LittleEndian(buffer.AsSpan()[29..33]);
        }
        else
        {
            FirstNode = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[1..5]);
            SecondNode = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[5..9]);
            RelationshipType = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[9..13]);
            FirstPrevRelId = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[13..17]);
            FirstNextRelId = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[17..21]);
            SecondPrevRelId = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[21..25]);
            SecondNextRelId = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[25..29]);
            NextPropId = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[29..33]);
        }
    }
}