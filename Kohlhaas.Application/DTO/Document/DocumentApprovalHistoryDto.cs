using Kohlhaas.Application.DTO.Document.Supporting;

namespace Kohlhaas.Application.DTO.Document;

public record DocumentApprovalHistoryDto
{
    public Guid DocumentId { get; init; }
    public string ControlNumber { get; init;  } = string.Empty;
    public string Title { get; init;  } = string.Empty;
    public List<ApprovalHistoryDto> Approvals { get; init; } = [];
}