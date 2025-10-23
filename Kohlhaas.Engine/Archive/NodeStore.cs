using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kohlhaas.Engine.Converters;
using Kohlhaas.Engine.Models;
using Microsoft.Extensions.Logging;

namespace Kohlhaas.Engine;

/// <summary>
// Represents the storage component responsible for managing nodes in the graph.
/// </summary>
public class NodeStore
{
    private readonly ILogger _logger;
    private const string NodeStoreFileName = "nodes.kohl";
    private const string DirectoryFolder = "Nodes";
    private const int BufferSize = 4096;
    private static ReadOnlySpan<byte> Utf8Bom => new byte[] { 0xEF, 0xBB, 0xBF };
    private readonly string _nodeStorePath;
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters =
        {
            
        }
    };

    public NodeStore(ILogger logger)
    {
        this._logger = logger;
        DataAccessUtility.CreateDirectoryIfMissing(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DirectoryFolder));
        _nodeStorePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
            DirectoryFolder, 
            NodeStoreFileName);
        DataAccessUtility.CreateFileIfMissing(_nodeStorePath);
    }
    
    

    public Result CreateNode(string vLevel, IDictionary<string, object> data, string tag, string fileName, string uniqueFileName)
    {
        var guid = Guid.CreateVersion7();
        try
        {
            using var sw = new StreamWriter(_nodeStorePath, true);
            
            using var writer = new Utf8JsonWriter(sw.BaseStream);
            writer.WriteStartObject();
            writer.WriteString("ID", guid);
            writer.WriteString("VLevel", vLevel);
            writer.WriteStartObject("Properties");
            foreach (var kv in data)
            {
                writer.WriteString(kv.Key, kv.Value.ToString());
            }
            writer.WriteEndObject();
            writer.WriteString("FileTag", tag);
            writer.WriteString("FileName", fileName);
            writer.WriteString("UniqueFileName", uniqueFileName);
            writer.WriteEndObject();
            sw.WriteLineAsync();
            writer.FlushAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Failed to create node. Exception encountered {Exception}", ex.Message);
            return Result.Failure(new Error(ex.HResult.ToString(), ex.Message));
        }
    }

    public async Task<Result<INode?>> ReadNode(Guid guid)
    {
        try
        {
            _logger.LogInformation("Reading node {Guid}", guid);
            await using var fs = new FileStream(_nodeStorePath, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize, FileOptions.Asynchronous);
            using var sr = new StreamReader(fs);
            while (await sr.ReadLineAsync() is { } line)
            {
                /*var bytes = Encoding.UTF8.GetBytes(line);
                Console.WriteLine($"Json line: {line}");*/
                _logger.LogInformation("Attempting to parse line: {Line}", line);
                var node = ParseNode(line);
                if (node is not null && node.Id.Equals(guid))
                {
                    _logger.LogInformation("Matching node found.");
                    return Result.Success<INode?>(node);
                }
            }
            _logger.LogInformation("No matching node found.");
            return Result.Failure<INode?>(new Error("0", $"Node with ID {guid} not found."));
        }
        catch (Exception ex)
        {
            return Result.Failure<INode?>(new Error(ex.HResult.ToString(), ex.Message));
        }
    }
    
    public async Task<Result> UpdateNode(INode node)
    {
        bool nodeFound = false;
        try
        {
            await using FileStream fs = new(_nodeStorePath, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            using StreamReader sr = new(fs, Encoding.UTF8);
            List<string> lines = [];
            while (await sr.ReadLineAsync() is { } line)
            {
                var foundNode = ParseNode(line);
                if (foundNode is null) break;
                if (foundNode.Id.Equals(node.Id))
                {
                    var updatedNode = foundNode.UpdateSelf(node);
                    string json = JsonSerializer.Serialize(updatedNode, Options);
                    lines.Add(json);
                    nodeFound = true;
                    continue;
                }

                lines.Add(line);
            }

            if (!nodeFound)
            {
                return Result.Failure<INode?>(new Error("0", $"Node with ID {node.Id} not found."));
            }

            await using StreamWriter sw = new(fs, Encoding.UTF8);
            sw.BaseStream.Seek(0, SeekOrigin.Begin);
            foreach (var line in lines)
            {
                await sw.WriteLineAsync(line);
            }
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure<INode?>(new Error(ex.HResult.ToString(), ex.Message));
        }
    }

    public void DeleteNode()
    {
    }

    private static INode? ParseNode(byte[] bytes, Guid guid)
    {
        var jsonReader = new Utf8JsonReader(bytes, isFinalBlock: false, state: default);
        while (jsonReader.Read())
        {
            if (jsonReader.TokenType != JsonTokenType.PropertyName || jsonReader.GetString() != "ID") continue;
            jsonReader.Read();
            var id = jsonReader.GetGuid();
            if (id == guid)
            {
                //Node node = new Node(id, string.Empty, new Dictionary<string, object>(), string.Empty, string.Empty, string.Empty);
                Dictionary<string, string> nodeData = new Dictionary<string, string>();

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = jsonReader.GetString();
                        jsonReader.Read();

                        switch (propertyName)
                        {
                            case "VLevel":
                                nodeData["VLevel"] = jsonReader.GetString() ?? string.Empty;
                                break;
                            case "FileTag":
                                nodeData["FileTag"] = jsonReader.GetString() ?? string.Empty;
                                break;
                            case "FileName":
                                nodeData["FileName"] = jsonReader.GetString() ?? string.Empty;
                                break;
                            case "UniqueFileName":
                                nodeData["UniqueFileName"] = jsonReader.GetString() ?? string.Empty;
                                break;
                            case "Data":
                                var builder = ImmutableDictionary.CreateBuilder<string, object>();
                                //builder.add()
                                //builder.ToImmutable()
                                break;
                        }
                    }
                }

                /*node.VLevel = nodeData["VLevel"];
                node.FileTag = nodeData["FileTag"];
                node.FileName = nodeData["FileName"];
                node.UniqueFileName = nodeData["UniqueFileName"];*/
                return new Node 
                {
                    Id = guid,
                    VLevel = nodeData["VLevel"],
                    FileTag = nodeData["FileTag"],
                    FileName = nodeData["FileName"],
                    UniqueFileName = nodeData["UniqueFileName"],
                    Properties = new Dictionary<string, object>().ToImmutableDictionary(),
                } ;
            }
            break;
        }
        return null;
    }
    
    private INode? ParseNode(string line)
    {
        var node = JsonSerializer.Deserialize<INode?>(line, options: Options);
        if (node is not null)
            return new Node
            {
                Id = node.Id,
                FileTag = node.FileTag,
                Properties = node.Properties,
                UniqueFileName = node.UniqueFileName,
                FileName = node.FileName,
                VLevel = node.VLevel,
            };
        _logger.LogInformation("Failed to parse node from line: {Line}", line);
        return null;
    }
}