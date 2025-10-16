using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Serialization;

public class NodeRecordSerializer : IRecordSerializer<NodeRecord>
{
    private const byte RecordSize = 13;
    private const byte InUsePos = 0;
    private const byte LabelsPos = 1;
    private const byte NextRelIdPos = 5;
    private const byte NextPropIdPos = 9;
    public NodeRecord Deserialize(byte[] bytes) => Deserialize(bytes.AsSpan());

    public NodeRecord Deserialize(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != RecordSize) throw new Exception($"Expected {RecordSize} bytes, got {bytes.Length}");
        
        return new NodeRecord(
            inUse: bytes[InUsePos],
            labels: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(LabelsPos, sizeof(uint))),
            nextRelId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(NextRelIdPos, sizeof(uint))),
            nextPropId: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(NextPropIdPos, sizeof(uint))));
    }

    public byte[] Serialize(NodeRecord record)
    {
        var data = new byte[RecordSize];
        data[InUsePos]  = record.InUse;
        BitConverter.GetBytes(record.Labels).CopyTo(data, LabelsPos);
        BitConverter.GetBytes(record.NextRelId).CopyTo(data, NextRelIdPos);
        BitConverter.GetBytes(record.NextPropId).CopyTo(data, NextPropIdPos);
        return data;
    }
}