namespace Kohlhaas.Application.DTO.Common;

public abstract record PagedResult<TSummaryDto>
{
    public List<TSummaryDto> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int CurrentPage { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
} 