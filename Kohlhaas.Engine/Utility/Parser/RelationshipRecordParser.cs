using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public class RelationshipRecordParser : IRecordParser<RelationshipRecord>
{
    public RelationshipRecord Parse(byte[] bytes) => Parse(bytes.AsSpan());

    public RelationshipRecord Parse(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length < 33)
            throw new ArgumentException("Insufficient data");

        return new RelationshipRecord(
            inUse: bytes[0],
            firstNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(1,4)),
            secondNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(5, 4)),
            relationshipType: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(9, 4)),
            firstPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(13, 4)),
            firstNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(17, 4)),
            secondPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(21, 4)),
            secondNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(25, 4)),
            nextPropId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(29, 4))
        );
    }
}