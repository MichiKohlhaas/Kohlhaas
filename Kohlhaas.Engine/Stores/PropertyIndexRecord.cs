namespace Kohlhaas.Engine.Stores;

/// <summary>
/// 
/// </summary>
/// <param name="inUse"></param>
/// <param name="RoN">Relationship or Node, 0 for R, 1 for N</param>
/// <param name="id">The relationship or node id</param>
/// <param name="recordLength"></param>
/// <param name="key"></param>
/// <param name="name"></param>
public readonly struct PropertyIndexRecord(byte inUse, byte RoN, int id, byte recordLength, byte key, ulong name)
{
    public byte InUse { get; init; } = inUse;
    public byte RoN { get; init; } = RoN;
    public int Id { get; init; } = id;
    public byte RecordLength { get; init; } = recordLength;
    public byte Key { get; init; } = key;
    public ulong Name { get; init; } = name;
}