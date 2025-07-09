using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public class NodeRecordParser : IRecordParser<NodeRecord>
{
    private const byte RecordSize = 13;
    private const byte InUsePos = 0;
    private const byte LabelsPos = 1;
    private const byte NextRelIdPos = 5;
    private const byte NextPropIdPos = 9;
    public NodeRecord ParseTo(byte[] bytes) => ParseTo(bytes.AsSpan());

    public NodeRecord ParseTo(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != RecordSize) throw new Exception($"Expected {RecordSize} bytes, got {bytes.Length}");
        
        return new NodeRecord(
            inUse: bytes[InUsePos],
            labels: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(LabelsPos, sizeof(uint))),
            nextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(NextRelIdPos, sizeof(uint))),
            nextPropId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(NextPropIdPos, sizeof(uint))));
    }
}