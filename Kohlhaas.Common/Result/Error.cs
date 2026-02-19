namespace Kohlhaas.Common.Result;

public readonly record struct Error(string Code, string Message)
{
    internal static readonly Error None = new(string.Empty, string.Empty);
    internal static Error NullValue = new("Error.NullValue", "A value was null.");

    public override string ToString()
    {
        return $"{Code}: {Message}";
    }
    
    public sealed record Query
    {
        public static Error ParseError(int line, int column, string message) => new("Error.Query.Parse", $"{line}:{column}: {message}");
        public static Error InvalidQuery(string message) => new Error("Error.Query.InvalidQuery", message); 
    }

    public sealed record Validation
    {
        public static Error ValidationError(string message) => new Error("Error.Validation.ValidationError", message);
    }

    public sealed record User
    {
        public static Error EmailAlreadyExists() => new("Error.User.EmailAlreadyExists", "A user with this email already exists.");
        public static Error InvalidEmail(string email) => new("Error.User.InvalidEmail", email);
        public static Error InvalidPassword(string password) => new("Error.User.InvalidPassword", password);
        public static Error Deactivated() => new("Error.User.Deactivated", "This account has been deactivated.");
        public static Error NotFound() => new("Error.User.NotFound", "User was not found.");
        public static Error InvalidCredentials() => new("Error.User.InvalidCredentials", "The email or password provided is incorrect.");
        public static Error DeactivateSelf() => new("Error.User.DeactivateSelf", "The same account cannot deactivate its self");
    }

    public sealed record Token
    {
        public static Error TokenUserIdNotFound() => new("Error.Token.TokenUserIdNotFound", "User ID was not found.");
        public static Error TokenUserRoleNotFound() => new("Error.Token.TokenUserRoleNotFound", "User role was not found.");
        public static Error TokenUserEmailNotFound() => new("Error.Token.TokenUserEmailNotFound", "User email was not found.");
    }

    public sealed record JwtToken
    {
        public static Error JwtTokenNotFound() => new("Error.Token.JwtTokenNotFound", "JWT refresh token was not found.");
        public static Error JwtTokenExpired() => new("Error.Token.JwtTokenExpired", "JWT refresh token has expired.");
    }

    public sealed record Authorization
    {
        public static Error Unauthorized() => new("Error.Authorization.Unauthorized", "Unauthorized action.");
        public static Error Forbidden() => new("Error.Authorization.Forbidden", "Forbidden action.");
    }

    public sealed record Project
    {
        public static Error ProjectIdNotFound() => new("Error.Project.ProjectIdNotFound", "Project ID was not found.");
        public static Error ProjectNameNotFound() => new("Error.Project.ProjectNameNotFound", "Project name was not found.");
        public static Error ProjectCodeNotUnique() => new("Error.Project.ProjectCodeNotUnique", "A project with this code already exists.");
        public static Error ProjectMemberNotFound() => new("Error.Project.ProjectMemberNotFound", "Project member was not found.");
        public static Error ProjectArchived() => new ("Error.Project.ProjectArchived", "This project has been archived.");
        public static Error NotProjectMember() => new ("Error.Project.NotProjectMember", "Not a member of the project.");
        public static Error ProjectOutstandingDocuments() => new ("Error.Project.ProjectOutstandingDocuments", "This project has outstanding documents.");
    }

    public sealed record ProjectMember
    {
        public static Error AlreadyProjectMember() => new ("Error.Project.AssignNewMember",
            "This user is already assigned to this project.");

        public static Error AlreadyDeactivated() => new ("Error.Project.AlreadyDeactivated", "This user is already deactivated.");
        public static Error AlreadyActive() => new ("Error.Project.AlreadyActive", "This user is already active.");
    }
}