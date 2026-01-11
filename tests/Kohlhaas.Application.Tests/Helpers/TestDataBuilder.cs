using Kohlhaas.Domain.Entities;
using Kohlhaas.Domain.Enums;

namespace Kohlhaas.Application.Tests.Helpers;

/// <summary>
/// Helper class to create test data objects
/// Centralizes test data creation for consistency across tests
/// </summary>
public static class TestDataBuilder
{
    /// <summary>
    /// Creates a user with specified properties, filling in defaults
    /// </summary>
    public static User CreateUser(
        Guid? id = null,
        string email = "test@test.com",
        string firstName = "Test",
        string lastName = "User",
        UserRole role = UserRole.User,
        bool isActive = true,
        string department = "Engineering")
    {
        return new User
        {
            Id = id ?? Guid.NewGuid(),
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Role = role,
            IsActive = isActive,
            PasswordHash = "hashed_password_" + Guid.NewGuid().ToString("N")[..8],
            Department = department,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates an admin user
    /// </summary>
    public static User CreateAdminUser(string email = "admin@test.com")
    {
        return CreateUser(
            email: email,
            firstName: "Admin",
            lastName: "User",
            role: UserRole.Admin,
            isActive: true,
            department: "IT");
    }

    /// <summary>
    /// Creates a project manager user
    /// </summary>
    public static User CreateProjectManagerUser(string email = "pm@test.com")
    {
        return CreateUser(
            email: email,
            firstName: "Project",
            lastName: "Manager",
            isActive: true,
            role: UserRole.ProjectManager);
    }

    /// <summary>
    /// Creates a project with specified properties
    /// </summary>
    public static Project CreateProject(
        Guid? id = null,
        string name = "Test Project",
        string code = "TEST-001",
        Guid? ownerId = null,
        string ownerName = "Test Owner",
        VModelPhase phase = VModelPhase.UserRequirements,
        bool isArchived = false,
        Guid? createdById = null)
    {
        return new Project
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Description = $"Description for {name}",
            Code = code,
            CurrentPhase = phase,
            OwnerId = ownerId ?? Guid.NewGuid(),
            OwnerName = ownerName,
            IsArchived = isArchived,
            DocumentsCount = 0,
            CreatedById = createdById,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a project member with specified properties
    /// </summary>
    public static ProjectMember CreateProjectMember(
        Guid? id = null,
        Guid? projectId = null,
        Guid? userId = null,
        string email = "member@test.com",
        ProjectRole role = ProjectRole.Member,
        bool isOwner = false,
        bool isActive = true)
    {
        return new ProjectMember
        {
            Id = id ?? Guid.NewGuid(),
            ProjectId = projectId ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            Email = email,
            Role = role,
            IsOwner = isOwner,
            IsActive = isActive,
            JoinedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a document with specified properties
    /// </summary>
    public static Document CreateDocument(
        Guid? id = null,
        Guid? projectId = null,
        string title = "Test Document",
        string controlNumber = "DOC-001",
        DocumentType type = DocumentType.Design,
        DocumentStatus status = DocumentStatus.Draft,
        VModelPhase phase = VModelPhase.UserRequirements,
        Guid? authorId = null)
    {
        return new Document
        {
            Id = id ?? Guid.NewGuid(),
            ProjectId = projectId ?? Guid.NewGuid(),
            Title = title,
            ControlNumber = controlNumber,
            Content = $"Content for {title}",
            Type = type,
            Status = status,
            Phase = phase,
            VersionNumber = 1,
            CreatedById = authorId,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };
    }
}
