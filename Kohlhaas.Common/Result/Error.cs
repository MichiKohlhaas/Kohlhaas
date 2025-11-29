using System.Runtime.InteropServices.JavaScript;

namespace Kohlhaas.Common.Result;

public readonly record struct Error(string Code, string Message)
{
    internal static readonly Error None = new(string.Empty, string.Empty);
    internal static Error NullValue = new("Error.NullValue", "A value was null.");

    public override string ToString()
    {
        return $"{Code}: {Message}";
    }
}