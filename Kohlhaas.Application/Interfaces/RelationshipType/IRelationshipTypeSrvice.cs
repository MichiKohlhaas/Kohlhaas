using Kohlhaas.Application.DTO.RelationshipType;
using Kohlhaas.Common.Result;

namespace Kohlhaas.Application.Interfaces.RelationshipType;

public interface IRelationshipTypeService
{
    /// <summary>
    /// Creates a user-defined relationship for linking documents.
    /// </summary>
    /// <param name="dto">The new relationship type data (<see cref="CreateRelationshipTypeDto"/>)</param>
    /// <returns>The created relationship type</returns>
    Task<Result<RelationshipTypeDto>> CreateRelationshipTypeAsync(CreateRelationshipTypeDto dto);

    /// <summary>
    /// Updates a user-defined relationship.
    /// </summary>
    /// <param name="dto">The update relationship type data (<see cref="UpdateRelationshipTypeDto"/>)</param>
    /// <returns>The updated relationship type</returns>
    Task<Result<RelationshipTypeDto>> UpdateRelationshipTypeAsync(UpdateRelationshipTypeDto dto);
    
    /// <summary>
    /// Activates or deactivates a relationship type.
    /// Inactive types don't appear in UI dropdowns but preserve existing links
    /// </summary>
    /// <param name="relationshipTypeId">The relationship type ID</param>
    /// <param name="isActive">True to activate, false to deactivate</param>
    /// <returns>The updated relationship type</returns>
    Task<Result<RelationshipTypeDto>> SetActiveStatusAsync(Guid relationshipTypeId, bool isActive);
    
    /// <summary>
    /// Get a specific relationship type by ID
    /// </summary>
    /// <param name="id">The relationship type ID</param>
    /// <returns>The relationship type</returns>
    Task<Result<RelationshipTypeDto>> GetRelationshipTypeAsync(Guid id);
    
    /// <summary>
    /// Gets all the relationship types. 
    /// </summary>
    /// <returns>A collection of all the relationship types</returns>
    Task<Result<IList<RelationshipTypeDto>>> GetAllRelationshipTypesAsync();
    
    /// <summary>
    /// Gets all active relationship types (for UI dropdowns).
    /// </summary>
    /// <returns>Collection of active relationship types</returns>
    Task<Result<IList<RelationshipTypeDto>>> GetActiveRelationshipTypesAsync();

    /// <summary>
    /// Delete the relationship type if it isn't system defined.
    /// </summary>
    /// <param name="relationshipTypeId">The relationship type ID</param>
    /// <returns><see cref="Result"/></returns>
    Task<Result> DeleteRelationshipTypeAsync(Guid relationshipTypeId);
}