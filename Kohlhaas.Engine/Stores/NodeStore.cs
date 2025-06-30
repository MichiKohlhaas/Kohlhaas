namespace Kohlhaas.Engine.Stores;

//May be used.
public readonly struct NodeStore
{
    public NodeRecord[] Records { get; init; } 
    public StoreHeader Header { get; init; }
    
    public NodeStore(StoreHeader header, NodeRecord record)
    {
        Header = header;
        Records = [record];
    }

    public NodeStore(StoreHeader header, NodeRecord[] records)
    {
        Header = header;
        Records = records;
    }
}