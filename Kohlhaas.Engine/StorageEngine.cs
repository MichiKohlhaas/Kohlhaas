using System.Collections.Immutable;
using System.Diagnostics;
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

    public async Task<Result<Node>> CreateNode(string collectionName, string nodeName, string vLevel, ImmutableDictionary<string, object> properties, string fileTag, string fileName, string uniqueFileName)
    {
        var collectionExists = DirectoryAccess.CheckCollectionExists(Path.Combine(_databaseFolder, collectionName));
        if (collectionExists.IsSuccess == false)
        {
            _logger.LogCritical("{CollectionName} does not exist. Error: {ErrorMessage}", collectionName, collectionExists.Error.Message);
            return Result.Failure<Node>(collectionExists.Error);
        }
        
        var node = new Node()
        {
            FileName = fileName,
            UniqueFileName = uniqueFileName,
            FileTag = fileTag,
            VLevel = vLevel,
            Id = IdGenerator(),
            Properties = properties,
        };
            
        
        return Result.Success<Node>(node);
    }
    

    private static Guid IdGenerator()
    {
        return Guid.NewGuid();
    }
}