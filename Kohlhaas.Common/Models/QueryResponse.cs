using Kohlhaas.Common.Interfaces;

namespace Kohlhaas.Common.Models;

public sealed class QueryResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public QueryResult Result { get; set; }
    public int? ErrorLine { get; set; }
    public int? ErrorColumn { get; set; }
    public DateTime Timestamp { get; set; }
}