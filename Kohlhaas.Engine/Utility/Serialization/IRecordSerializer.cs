namespace Kohlhaas.Engine.Utility.Serialization;

public interface IRecordSerializer<T> where T : struct
{
    T Deserialize(byte[] bytes);
    T Deserialize(ReadOnlySpan<byte> bytes);
    byte[] Serialize(T record);
}