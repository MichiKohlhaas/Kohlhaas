namespace Kohlhaas.Common.Result;

public readonly record struct Error(string Code, string Message)
{
    internal static readonly Error None = new(string.Empty, string.Empty);
    internal static Error NullValue = new("Error.NullValue", "A value was null.");

    public override string ToString()
    {
        return $"{Code}: {Message}";
    }
    
    public record Query
    {
        public static Error ParseError(int line, int column, string message) => new("Error.Query.Parse", $"{line}:{column}: {message}");
        public static Error InvalidQuery(string message) => new Error("Error.Query.InvalidQuery", message); 
    }

    public record User
    {
        public static Error EmailAlreadyExists() => new("Error.User.EmailAlreadyExists", "A user with this email already exists.");
        public static Error InvalidEmail(string email) => new("Error.User.InvalidEmail", email);
        public static Error InvalidPassword(string password) => new("Error.User.InvalidPassword", password);
    }
}