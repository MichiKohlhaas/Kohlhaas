namespace Kohlhaas.Engine.Utility.Parser;

public interface IRecordParser<T> where T : struct
{
    T ParseTo(byte[] bytes);
    T ParseTo(ReadOnlySpan<byte> bytes);
    byte[] ParseFrom(T record);
}