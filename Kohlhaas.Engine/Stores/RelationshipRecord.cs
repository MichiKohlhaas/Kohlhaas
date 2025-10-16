namespace Kohlhaas.Engine.Stores;

/// <summary>
/// inUse
///  ↓   | FirstNode   | | SecondNode  | | RelType     | |1stPrevRelId | |1stNextRelId | |2ndPrevRelId | |2ndNextRelId | | NextPropId  |
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
}