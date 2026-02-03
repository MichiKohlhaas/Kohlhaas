using Kohlhaas.Application.DTO.Document;
using Kohlhaas.Application.DTO.Document.Supporting;
using Kohlhaas.Application.Interfaces.Document;
using Kohlhaas.Common.Result;

namespace Kohlhaas.Application.Services;

public class DocumentService : IDocumentService
{
    public Task<Result<DocumentDetailDto>> CreateDocumentAsync(Guid projectId, CreateDocumentDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentDetailDto>> LinkDocumentAsync(Guid projectId, DocumentLinkDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentDetailDto>> UploadDocumentFileAsync(Guid projectId, UploadFileDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<Stream>> DownloadDocumentFileAsync(Guid projectId, Guid documentId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentDetailDto>> UpdateDocumentAsync(Guid projectId, UpdateDocumentDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> SubmitForReviewAsync(Guid projectId, SubmitForReviewDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ReviewDocumentAsync(Guid projectId, ApprovalDecisionDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result> BulkReviewDocumentsAsync(Guid projectId, BulkOperationDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentDetailDto>> UnlockDocumentAsync(Guid projectId, UnlockDocumentDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentDetailDto>> GetDocumentAsync(Guid projectId, Guid documentId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<PagedDocumentsDto>> GetDocumentsAsync(Guid projectId, DocumentFilterDto dto)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentForReviewDto>> GetDocumentForReviewAsync(Guid projectId, Guid documentId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<DocumentForReviewDto>>> GetAssignedReviewDocumentsAsync(Guid reviewerId, Guid? projectId = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<DocumentSummaryDto>>> GetAllDocumentsByAsync(Guid authorId, Guid? projectId = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<LinkedDocumentDto>>> GetDocumentLinksAsync(Guid projectId, Guid documentId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IList<DocumentForReviewDto>>> GetOverdueDocumentsAsync(Guid reviewerId, Guid? projectId = null)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentApprovalHistoryDto>> GetApprovalHistoryAsync(Guid projectId, Guid documentId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<DocumentDetailDto>> MarkDocumentObsoleteAsync(Guid projectId, Guid documentId, string reason)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteDocumentLinkAsync(Guid projectId, Guid linkId)
    {
        throw new NotImplementedException();
    }
}