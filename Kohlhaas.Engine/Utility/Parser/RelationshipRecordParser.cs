using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public static class RelationshipRecordParser
{
    public static RelationshipRecord Parse(byte[] bytes)
    {
        if (bytes.Length < 33)
            throw new ArgumentException("Insufficient data");

        return new RelationshipRecord(
            inUse: bytes[0],
            firstNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(1,4)),
            secondNode: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(5, 4)),
            relationshipType: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(9, 4)),
            firstPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(13, 4)),
            firstNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(17, 4)),
            secondPrevRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(21, 4)),
            secondNextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(25, 4)),
            nextPropId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.AsSpan(29, 4))
        );
    }
    
    public static RelationshipRecord Parse(BinaryReader reader)
    {
        return new RelationshipRecord(
            inUse: reader.ReadByte(),
            firstNode: reader.ReadUInt32(),
            secondNode: reader.ReadUInt32(),
            relationshipType: reader.ReadUInt32(),
            firstPrevRelId: reader.ReadUInt32(),
            firstNextRelId: reader.ReadUInt32(),
            secondPrevRelId: reader.ReadUInt32(),
            secondNextRelId: reader.ReadUInt32(),
            nextPropId: reader.ReadUInt32()
        );
    }
}