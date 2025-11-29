using Kohlhaas.Common.Models;
namespace Kohlhaas.Common.Interfaces;

public interface IQueryExecutor
{
    Task<QueryResponse> ExecuteQueryAsync(QueryRequest request, CancellationToken cancellationToken = default);
}