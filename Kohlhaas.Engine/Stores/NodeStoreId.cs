namespace Kohlhaas.Engine.Stores;

public readonly struct NodeStoreId(byte storeId, byte[] guid)
{
    public byte StoreId { get; } = storeId;
    public byte[] Guid { get; } = guid;
}