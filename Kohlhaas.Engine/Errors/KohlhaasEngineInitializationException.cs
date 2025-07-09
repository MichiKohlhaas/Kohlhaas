namespace Kohlhaas.Engine.Errors;

public class KohlhaasEngineInitializationException : Exception
{
    public KohlhaasEngineInitializationException()
        : base("Storage engine failed to initialize")
    {
    }

    public KohlhaasEngineInitializationException(string message)
        : base(message)
    {
    }

    public KohlhaasEngineInitializationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}