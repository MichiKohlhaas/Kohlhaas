using System.Text;
using Kohlhaas.Engine.Layout.RecordStorage;
using Kohlhaas.Common.Models;
using Kohlhaas.Common.Interfaces;
using Kohlhaas.Engine.Stores;
using Microsoft.Extensions.Logging;

namespace Kohlhaas.Engine;

public sealed class StorageEngine
{
    // API:
    // Get DB state
    // Create/drop v-levels
    
    private const string DirectoryFolder = "Nodes"; 
    private const byte LabelBlockSize = 20;
    private readonly ILogger _logger;
    private readonly MasterStoreRecord? _masterStoreRecord;
    private readonly string _databaseFolder;
    
    public List<string> Collections;
    
    public StorageEngine(ILogger? logger)
    {
        _logger = logger ?? new LoggerFactory().CreateLogger<StorageEngine>();
        _databaseFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), DirectoryFolder);
        
        var msrResult = DirectoryAccess.TopLevelInfo(_databaseFolder);
        if (msrResult.IsSuccess == false || msrResult.Value is null)
        {
            throw new KohlhaasEngineInitializationException($"Failed to initialize KohlhaasEngine. Message: {msrResult.Error.Message}. Code: {msrResult.Error.Code}");
        }
        _masterStoreRecord = msrResult.Value;
        
        Collections = _masterStoreRecord?.Collections ?? [];
    }
    /// <summary>
    /// Update based on the 92 bytes of data.
    /// </summary>
    public void UpdateMsr()
    {
        
    }

    public async Task<Result> CreateCollection(string collectionName)
    {
        return await DirectoryAccess.CreateCollection(_databaseFolder, collectionName, _masterStoreRecord!);
    }

    public Result DeleteCollection(string collectionName)
    {
        return DirectoryAccess.DeleteCollection(_databaseFolder, collectionName, _masterStoreRecord!);
    }

    internal Result<(INode node, NodeRecord record)> CreateNode(string collectionName, Node node)
    {
        //check that labels.Length <= 3 elsewhere?
        
        var collectionPath = Path.Combine(_databaseFolder, collectionName);
        var collectionExists = DirectoryAccess.CheckCollectionExists(collectionPath);
        if (collectionExists.IsSuccess == false)
        {
            _logger.LogCritical("{CollectionName} does not exist. Error: {ErrorMessage}", collectionName, collectionExists.Error.Message);
            return Result.Failure<(INode node, NodeRecord record)>(collectionExists.Error);
        }
        
        //write node to db
        //write labels
        //List<Task> tasks = [];
        Result<(LabelRecord, long)>? labelResult = null;
        if (node.Labels is not null)
        {
            List<byte> serializedLabels = [];
            foreach (var label in node.Labels)
            {
                serializedLabels.AddRange(Encoding.UTF8.GetBytes(label));
            }
            labelResult = DataAccess.CreateLabelRecord(Path.Combine(collectionPath, RecordDatabaseFileNames.LabelsStore), serializedLabels.ToArray());
            if (labelResult.IsSuccess is false) return Result.Failure<(INode node, NodeRecord record)>(labelResult.Error);
        }
        
        Result<PropertyRecord>? propertyResult = null;
        if (node.Properties is not null)
        {
            propertyResult = DataAccess.CreatePropertyRecord(Path.Combine(collectionPath, RecordDatabaseFileNames.PropertyStore), node.Properties);
            if (propertyResult.IsSuccess is false) return Result.Failure<(INode node, NodeRecord record)>(propertyResult.Error);
        }
        
        //write node
        var labelsId = labelResult is { IsSuccess : true } ? labelResult.Value.Item2 < uint.MaxValue ? (uint)labelResult.Value.Item2 : throw new Exception($"Label ID not found. ID: {labelResult.Value.Item2}") : 0;
        
        var nodeResult = DataAccess.CreateNodeRecord(Path.Combine(collectionPath, RecordDatabaseFileNames.NodesStore), labelsId, propertyResult);
        
        if (nodeResult.IsSuccess is false) return Result.Failure<(INode node, NodeRecord record)>(nodeResult.Error);
        
        node.UpdateSelf(node with { Id = (int)(nodeResult.Value.nodeId) });
        return Result.Success<(INode node, NodeRecord record)>((node, nodeResult.Value.record));
    }
    

    /*private static Guid IdGenerator()
    {
        return Guid.NewGuid();
    }*/
}