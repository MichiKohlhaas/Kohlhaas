using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Serialization;

public class LabelSerializer : IRecordSerializer<LabelRecord>
{
    private const byte LabelSize = 65;
    private const byte InUsePos = 0;
    private const byte ReservedSpacePos = 1;
    private const byte LabelDataPos = 5;
    
    public LabelRecord Deserialize(byte[] bytes) => Deserialize(bytes.AsSpan());

    public LabelRecord Deserialize(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length != LabelSize)  throw new Exception($"Expected {LabelSize} bytes, got {bytes.Length}");

        return new LabelRecord(
            inUse: bytes[InUsePos],
            reservedSpace: BinaryPrimitives.ReadUInt32LittleEndian(bytes.Slice(ReservedSpacePos,sizeof(uint))),
            labelData: bytes[LabelDataPos..].ToArray()
        );
    }

    public byte[] Serialize(LabelRecord record)
    {
        var data = new byte[LabelSize];
        data[InUsePos] = record.InUse;
        BitConverter.GetBytes(record.ReservedSpace).CopyTo(data, ReservedSpacePos);
        record.LabelData.CopyTo(data, LabelDataPos);
        return data;
    }
}