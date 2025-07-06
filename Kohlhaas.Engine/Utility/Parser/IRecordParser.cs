namespace Kohlhaas.Engine.Utility.Parser;

public interface IRecordParser<T> where T : struct
{
    T Parse(byte[] bytes);
    T Parse(ReadOnlySpan<byte> bytes);
}