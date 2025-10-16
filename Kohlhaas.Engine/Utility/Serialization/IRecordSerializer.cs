namespace Kohlhaas.Engine.Utility.Serialization;

/// <summary>
/// Going forward, I will assume that binary data is serialized in little Endian.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IRecordSerializer<T> where T : struct
{
    T Deserialize(byte[] bytes);
    T Deserialize(ReadOnlySpan<byte> bytes);
    byte[] Serialize(T record);
}