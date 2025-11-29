using Kohlhaas.Common.Enums;
namespace Kohlhaas.Common.Models;

public class QueryResult
{
    public Node? Node { get; set; }
    public Relationship? Relationship { get; set; }
    public Graph? Graph { get; set; }
    public QueryResultType Type { get; set; }
    
}