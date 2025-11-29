using System.Collections.Immutable;
using System.Diagnostics;
using Kohlhaas.Common.Enums;
using Kohlhaas.DB;
using Kohlhaas.Common.Interfaces;
using Kohlhaas.Common.Models;



namespace Kohlhaas.DB;

public class QueryExecutorService(ILogger<QueryExecutorService> logger) : IQueryExecutor
{
    private readonly ILogger<QueryExecutorService> _logger = logger;

    public async Task<QueryResponse> ExecuteQueryAsync(QueryRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Query == null)
        {
            return new QueryResponse()
            {
                Success = false,
                Message = "Query was empty",
            };
        }
        try
        {
            var parser = new Parser.Parser(request.Query);
            var scriptRoot = parser.GetTokenTree();
#if DEBUG
            DumpTokenTree(scriptRoot);
#endif
            // TODO: parse the actual incoming query
            return new QueryResponse()
            {
                Success = true,
                Message = "Query executed successfully",
                Result = new QueryResult()
                {
                    Type = QueryResultType.Node,
                    Node = new Node()
                    {
                        Labels = ["Dummy label 1", "Dummy label 2", "Dummy label 3"],
                        Properties = new Dictionary<string, object>()
                        {
                            { "Dummy property key", "Dummy property value" }
                        }.ToImmutableDictionary()
                    }
                },
            };
        }
        catch (LineColumnException ex)
        {
            _logger.LogError("[" + ex.Line + "/" + ex.Column + "] ERROR: " + ex.Message);
            Debug.WriteLine("[" + ex.Line + "/" + ex.Column + "] ERROR: " + ex.Message);

            return new QueryResponse()
            {
                Success = false,
                Message = ex.Message,
                ErrorColumn = ex.Column,
                ErrorLine = ex.Line,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return new QueryResponse()
            {
                Success = false,
                Message = ex.Message,
            };
        }
    }
    
    private static void DumpTokenNode(Token token, int indent)
    {
        Debug.WriteLine(new string(' ', indent) + token.LineColumnText + " " + token.Type + " '" + token.Value + "'");
        var child = token.Childs;
        while (child != null)
        {
            DumpTokenNode(child, indent + 2);
            child = child.Sibling;
        }
    }

    private static void DumpTokenTree(Token root)
    {
        DumpTokenNode(root, 0);
    }
}