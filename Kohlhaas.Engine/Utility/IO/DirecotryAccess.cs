using Kohlhaas.Engine.Layout.RecordStorage;
using Kohlhaas.Engine.Stores;
using Kohlhaas.Engine.Utility.Serialization;

namespace Kohlhaas.Engine.Utility.IO;

public static class DirectoryAccess
{
    private const string MsrFile = "Kohlhaas.MSR.db";
    private const byte MsrSize = 92;
    
    internal static Result<MasterStoreRecord> TopLevelInfo(string path)
    {
        var msrFilePath = Path.Combine(path, MsrFile);
        try
        {
            // First time
            if (Directory.Exists(path) == false)
            {
               
                Directory.CreateDirectory(path);
                File.Create(msrFilePath, MsrSize);
                return Result.Success(new MasterStoreRecord());
            }
            
            // directory exists
            // but MSR doesn't...?
            if (File.Exists(msrFilePath) == false)
            {
                File.Create(msrFilePath, MsrSize);
                return Result.Success(new MasterStoreRecord());
            }
            
            var subDirectories = Directory.GetDirectories(path);
            return DataAccess.ReadStreamOperation(msrFilePath, reader =>
            {
                var msrData = reader.ReadBytes(MsrSize);
                var serializer = new MasterStoreRecordSerializer();
                var msr = serializer.Deserialize(msrData);
                //var msr = new MasterStoreRecord(subDirectories.ToList(), msrData);
                return Result.Success(msr);
            });
        }
        catch (Exception e)
        {
            return Result.Failure<MasterStoreRecord>(new Error(e.HResult.ToString(), e.Message));
        }
    }

    internal static async Task<Result> CreateCollection(string path, string collectionName, MasterStoreRecord msr)
    {
        if (Directory.Exists(path) == false)
        {
            return Result.Failure(new Error("Error code: CODE","Top level directory does not exist."));
        }

        try
        {
            _ = Directory.CreateDirectory(Path.Combine(path, collectionName));
            msr.Collections.Add(collectionName);
            await CreateCollectionStores();
            return Result.Success();

        }
        catch (Exception e)
        {
            return Result.Failure(new Error(e.HResult.ToString(), e.Message));
        }
    }

    public static Result DeleteCollection(string path, string collectionName, MasterStoreRecord msr)
    {
        if (Directory.Exists(path) == false)
        {
            return Result.Failure(new Error("Error code: CODE","Directory does not exist."));
        }

        try
        {
            Directory.Delete(Path.Combine(path, collectionName), true);
            msr.Collections.Remove(collectionName);
            return Result.Success();
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(e.HResult.ToString(), e.Message));
        }
    }

    private static async Task CreateCollectionStores()
    {
        await DataAccess.CreateEmptyStoresAsync(RecordDatabaseFileNames.FileNames);
    }
}