using Kohlhaas.Common.Models;
using Kohlhaas.Common.Result;
namespace Kohlhaas.Common.Interfaces;

public interface IQueryExecutor
{
    Task<Result<QueryResponse>> ExecuteQueryAsync(QueryRequest request, CancellationToken cancellationToken = default);
}