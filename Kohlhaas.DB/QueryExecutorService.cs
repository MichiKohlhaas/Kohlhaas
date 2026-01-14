using System.Collections.Immutable;
using System.Diagnostics;
using Kohlhaas.Common.Enums;
using Kohlhaas.DB;
using Kohlhaas.Common.Interfaces;
using Kohlhaas.Common.Models;
using Kohlhaas.Common.Result;
using Error = Kohlhaas.Common.Result.Error;


namespace Kohlhaas.DB;

public class QueryExecutorService(ILogger<QueryExecutorService> logger) : IQueryExecutor
{
    public async Task<Result<QueryResponse>> ExecuteQueryAsync(QueryRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Query == null)
        {
            return Result.Success(new QueryResponse()
            {
                Success = false,
                Message = "Query was empty",
            });
        }
        
        var tokenTreeResult = TryParseQuery(request.Query);
        if (tokenTreeResult.IsSuccess is false) return Result.Failure<QueryResponse>(tokenTreeResult.Error);
        
        // TODO: parse the actual incoming query
        var startingToken = tokenTreeResult.Value.Childs;
        switch (startingToken.Type)
        {
            case TokenEnum.CreateNodeCommand:
                var createNode = new Node();
                ParseTokens(startingToken.Childs, createNode);
                break;
            default:
                break;
        }
        return Result.Success(new QueryResponse()
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
        });
    }

    private void ParseTokens(Token tokenTree, Node createNode)
    {
        logger.LogInformation("Starting token tree parse.");
        switch (tokenTree.Type)
        {
            case TokenEnum.CreateNodeCommand:
                break;
            case TokenEnum.NodeDefinition:
                ParseTokens(tokenTree.Childs, createNode);
                break;
            case TokenEnum.LabelArray:
                
            case TokenEnum.PropertyList:
            
            case TokenEnum.Property:
            
                
                break;
            default:
                break;
        }

    }

    private Result<Token> TryParseQuery(string query)
    {
        try
        {
            var parser = new Parser.Parser(query);
            var tokenTree = parser.GetTokenTree();
#if DEBUG
            DumpTokenTree(tokenTree);
#endif
            return Result.Success(tokenTree);
        }
        catch (LineColumnException lcEx)
        {
            logger.LogWarning(lcEx, "Error parsing query at: {Line}/{Column}: {Message}", lcEx.Line, lcEx.Column,
                lcEx.Message);
            return Result.Failure<Token>(Error.Query.ParseError(lcEx.Line, lcEx.Column, lcEx.Message));
        }
        catch (Exception ex)
        {
            logger.LogError("Unknown error while parsing query: {Message}", ex.Message);
            return Result.Failure<Token>(new Error("Error.Uknown", ex.Message));
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