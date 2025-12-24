namespace Kohlhaas.Application.DTO.Document.Supporting;

/// <summary>
/// Used in <see cref="DocumentDetailDto"/>. Represents the document to which it links.
/// </summary>
/// <param name="Id"></param>
/// <param name="ContentNumber"></param>
/// <param name="Title"></param>
/// <param name="RelationshipTypeId"></param>
/// <param name="RelationshipTypeName"></param>
/// <param name="RelationshipTypeIcon"></param>
/// <param name="RelationshipTypeColour"></param>
public record LinkedDocumentDto(
    Guid Id,
    string ContentNumber,
    string Title,
    Guid RelationshipTypeId,
    string RelationshipTypeName,
    string RelationshipTypeIcon,
    string? RelationshipTypeColour
);