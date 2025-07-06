using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public class NodeRecordParser : IRecordParser<NodeRecord>
{
    public NodeRecord Parse(byte[] bytes) => Parse(bytes.AsSpan());

    public NodeRecord Parse(ReadOnlySpan<byte> bytes)
    {
        return new NodeRecord(0, 0, 0, 0);
    }
}