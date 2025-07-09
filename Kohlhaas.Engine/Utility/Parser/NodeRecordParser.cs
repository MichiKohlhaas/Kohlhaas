using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public class NodeRecordParser : IRecordParser<NodeRecord>
{
    public NodeRecord ParseTo(byte[] bytes) => ParseTo(bytes.AsSpan());

    public NodeRecord ParseTo(ReadOnlySpan<byte> bytes)
    {
        return new NodeRecord(0, 0, 0, 0);
    }
}