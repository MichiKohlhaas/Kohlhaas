namespace Kohlhaas.Parser;

public sealed class LineColumnException(int line, int column, string message) : Exception(message)
{
    public int Line { get; private set; } = line;

    public int Column { get; private set; } = column;
}