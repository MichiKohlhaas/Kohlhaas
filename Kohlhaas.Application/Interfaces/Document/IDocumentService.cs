using Kohlhaas.Application.DTO.Document;
using Kohlhaas.Application.DTO.Document.Supporting;
using Kohlhaas.Common.Result;

namespace Kohlhaas.Application.Interfaces.Document;

public interface IDocumentService
{
    /// <summary>
    /// Create a document for a specific project.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">Document data</param>
    /// <returns>The created document</returns>
    Task<Result<DocumentDetailDto>> CreateDocumentAsync(Guid projectId, CreateDocumentDto dto);

    /// <summary>
    /// Link one document to another, described by their relationship.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">The document link data (<see cref="DocumentLinkDto"/>)</param>
    /// <returns>The updated document</returns>
    Task<Result<DocumentDetailDto>> LinkDocumentAsync(Guid projectId, DocumentLinkDto dto);

    /// <summary>
    /// Upload a file attached to a document. 
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">File to upload and metadata</param>
    /// <returns>Result upon success</returns>
    Task<Result<DocumentDetailDto>> UploadDocumentFileAsync(Guid projectId, UploadFileDto dto);

    /// <summary>
    /// Download a file attached to a document.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="documentId">The document the file is attached to</param>
    /// <returns>Byte array of the document</returns>
    Task<Result<Stream>> DownloadDocumentFileAsync(Guid projectId, Guid documentId);

    /// <summary>
    /// Update a document.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">The updated document data (<see cref="UpdateDocumentDto"/>)</param>
    /// <returns>The updated document</returns>
    Task<Result<DocumentDetailDto>> UpdateDocumentAsync(Guid projectId, UpdateDocumentDto dto);

    /// <summary>
    /// Submit a document for review, assigning the reviewer(s).
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">The document to be reviewed (<see cref="SubmitForReviewDto"/>)</param>
    /// <returns><see cref="Result"/></returns>
    Task<Result> SubmitForReviewAsync(Guid projectId, SubmitForReviewDto dto);

    /// <summary>
    /// Submits a review decision (approve, reject, request revision) for a document.
    /// Approved: Document status -> approved (if all reviewers approve)
    /// Rejected: Document status -> draft
    /// Needs Revision: Document status -> draft
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">Review decision (<see cref="ApprovalDecisionDto"/>)</param>
    /// <returns><see cref="Result"/></returns>
    Task<Result> ReviewDocumentAsync(Guid projectId, ApprovalDecisionDto dto);

    /// <summary>
    /// Operation to bulk approve or reject documents, making it easier to review several at once.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">Bulk review data (<see cref="BulkOperationDto"/>)</param>
    /// <returns></returns>
    Task<Result> BulkReviewDocumentsAsync(Guid projectId, BulkOperationDto dto);

    /// <summary>
    /// Unlock a document for editing after it has been approved. Also unlocks
    /// Would cover un-submitting a document for review too.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="dto">The document to unlock (<see cref="UnlockDocumentDto"/>)</param>
    /// <returns>The unlocked document for editing</returns>
    Task<Result<DocumentDetailDto>> UnlockDocumentAsync(Guid projectId, UnlockDocumentDto dto);

    /// <summary>
    /// Get a specific document.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="documentId">The document to retrieve</param>
    /// <returns>The document</returns>
    Task<Result<DocumentDetailDto>> GetDocumentAsync(Guid projectId, Guid documentId);

    /// <summary>
    /// Gets a collection of documents based on a filter.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="dto">Rules for filtering the documents (<see cref="DocumentFilterDto"/>)</param>
    /// <returns>The paginated document collection</returns>
    Task<Result<PagedDocumentsDto>> GetDocumentsAsync(Guid projectId, DocumentFilterDto dto);

    /// <summary>
    /// Gets a specific document that is marked for review.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="documentId">The document ID</param>
    /// <returns>The document for review</returns>
    Task<Result<DocumentForReviewDto>> GetDocumentForReviewAsync(Guid projectId, Guid documentId);

    /// <summary>
    /// Gets a documents assigned to a reviewer.
    /// </summary>
    /// <param name="reviewerId">The reviewer's ID</param>
    /// <param name="projectId">Optional project ID filter</param>
    /// <returns>Collection of documents assigned to reviewer</returns>
    Task<Result<IList<DocumentForReviewDto>>> GetAssignedReviewDocumentsAsync(Guid reviewerId, Guid? projectId = null);

    /// <summary>
    /// Gets all documents created by a contributor.
    /// </summary>
    /// <param name="authorId">The ID of the contributor</param>
    /// <param name="projectId">Option project ID filter</param>
    /// <returns>The collection of documents created by the contributor</returns>
    Task<Result<IList<DocumentSummaryDto>>> GetAllDocumentsByAsync(Guid authorId, Guid? projectId = null);

    /// <summary>
    /// Gets the links that a document has. Document detail already has all the links, this is for
    /// specific case where only the links are needed.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="documentId">The document ID</param>
    /// <returns>A collection of links from a document</returns>
    Task<Result<IList<LinkedDocumentDto>>> GetDocumentLinksAsync(Guid projectId, Guid documentId);

    /// <summary>
    /// Gets all overdue-for-review documents assigned to reviewer.
    /// </summary>
    /// <param name="reviewerId">Reviewer's ID</param>
    /// <param name="projectId">Optional project ID filter</param>
    /// <returns>List of documents for review</returns>
    Task<Result<IList<DocumentForReviewDto>>> GetOverdueDocumentsAsync(Guid reviewerId, Guid? projectId = null);

    /// <summary>
    /// Gets the approval history of a document.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="documentId">The document ID</param>
    /// <returns>The approval history data</returns>
    Task<Result<DocumentApprovalHistoryDto>> GetApprovalHistoryAsync(Guid projectId, Guid documentId);

    /// <summary>
    /// Mark a document when it is obsolete. Becomes read-only.
    /// PM, owner, or admin only.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="documentId">The document ID</param>
    /// <param name="reason">The reason the document was marked obsolete</param>
    /// <returns>The archived document</returns>
    Task<Result<DocumentDetailDto>> MarkDocumentObsoleteAsync(Guid projectId, Guid documentId, string reason);

    /// <summary>
    /// Deletes the relationship link between two documents.
    /// </summary>
    /// <param name="projectId">The project ID</param>
    /// <param name="linkId">Link ID</param>
    /// <returns><see cref="Result"/></returns>
    Task<Result> DeleteDocumentLinkAsync(Guid projectId, Guid linkId);
}