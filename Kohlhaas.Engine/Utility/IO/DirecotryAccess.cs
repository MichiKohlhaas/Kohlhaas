using Kohlhaas.Engine.Interfaces;
using Kohlhaas.Engine.Layout.RecordStorage;
using Kohlhaas.Engine.Models;
using Kohlhaas.Engine.Stores;
using Kohlhaas.Engine.Utility.Factories;
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
            var createdDirectory = Directory.CreateDirectory(Path.Combine(path, collectionName));
            msr.Collections.Add(collectionName);
            var result = await CreateCollectionStores(createdDirectory.FullName);
            return result.IsSuccess ? Result.Success() : result;
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(e.HResult.ToString(), e.Message));
        }
    }

    internal static Result<FileInfo[]> ReadDirectory(string path)
    {
        if (Directory.Exists(path) == false)
        {
            return Result.Failure<FileInfo[]>(new Error("Error code: CODE", "Directory does not exist."));
        }

        try
        {
            var directoryInfo = new DirectoryInfo(path);
            return Result.Success(directoryInfo.GetFiles());
        }
        catch (Exception e)
        {
            return Result.Failure<FileInfo[]>(new Error(e.HResult.ToString(), e.Message));
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

    public static Result<bool> CheckCollectionExists(string path)
    {
        return  Directory.Exists(path) ? Result.Success(true) : Result.Failure<bool>(new Error("Error code: CODE", "Directory does not exist."));
    }

    private static async Task<Result> CreateCollectionStores(string path)
    {
        var factory = new StoreHeaderFactory();
        var tasks = RecordDatabaseFileNames.StoreFiles.Select(async kvp =>
        {
            var config = new StoreHeaderConfiguration
            {
                FormatVersion = 0,
                FileTypeId = (byte)kvp.Key,
                FileVersion = 1,
                MagicNumber = 7,
                RecordSize = 15,
                Encoding = 0,
                AdditionalParameters = 0,
                TransactionLogSequence = 0,
                Checksum = 0,
                Lkgs = 0,
                Reserved = 0
            };
            var storeHeader = factory.CreateStoreHeader(config);
            return await DataAccess.WriteStoreHeaderAsync(path, storeHeader);
        }).ToArray();
        
        var results = await Task.WhenAll(tasks);
        var failures = results.Where(r => r.IsSuccess == false).ToList();
        
        if (failures.Count == 0) return Result.Success();
        
        var failedStore = string.Join(", ", failures.Select(s => s.Error.Message));
        return Result.Failure(new Error("Error code: CODE", failedStore));
    }
}