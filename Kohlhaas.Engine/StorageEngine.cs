using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Text;
using Kohlhaas.Engine.Layout.RecordStorage;
using Kohlhaas.Engine.Models;
using Kohlhaas.Engine.Stores;
using Microsoft.Extensions.Logging;

namespace Kohlhaas.Engine;

public class StorageEngine
{
    // API:
    // Get DB state
    // Create/drop v-levels
    
    private const string DirectoryFolder = "Nodes";
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

    public async Task<Result<INode>> CreateNode(string collectionName, string[]? labels, ImmutableDictionary<string, object>? properties, List<IRelationship>? relationships)
    {
        //check that labels.Length <= 3 elsewhere?
        
        var collectionPath = Path.Combine(_databaseFolder, collectionName);
        var collectionExists = DirectoryAccess.CheckCollectionExists(collectionPath);
        if (collectionExists.IsSuccess == false)
        {
            _logger.LogCritical("{CollectionName} does not exist. Error: {ErrorMessage}", collectionName, collectionExists.Error.Message);
            return Result.Failure<INode>(collectionExists.Error);
        }

        var node = new Node()
        {
            Id = IdGenerator(),
            Labels = labels,
            Relationships = relationships,
            Properties = properties,
        };
        
        //write node to db
        //write labels
        var serializedLabels = Span<byte>.Empty;
        //List<Task> tasks = [];
        Result<LabelRecord>? labelResult;
        if (labels is not null)
        {
            const int labelBlockSize = 20;
            for (var i = 0; i < labels.Length; i++)
            {
                Encoding.Unicode.GetBytes(labels[i]).CopyTo(serializedLabels[(i * labelBlockSize)..]);
            }
            
            labelResult = DataAccess.CreateLabelRecord(Path.Combine(_databaseFolder, RecordDatabaseFileNames.LabelsStore), serializedLabels.ToArray());
            if (labelResult.IsSuccess is false) return Result.Failure<INode>(labelResult.Error);
        }
        
        Result<PropertyRecord>? propertyResult;
        if (properties is not null)
        {
            DataAccess.CreatePropertyRecord(_databaseFolder, properties);
        }
        //write node
        
        return Result.Success<INode>(node);
    }
    

    private static Guid IdGenerator()
    {
        return Guid.NewGuid();
    }
}