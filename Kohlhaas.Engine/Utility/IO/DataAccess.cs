using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using Kohlhaas.Engine.Stores;
using Kohlhaas.Engine.Utility.Serialization;

namespace Kohlhaas.Engine.Utility.IO;

internal static class DataAccess
{
    private const byte StoreHeaderSize = 15;
    private const byte StreamOffset = 0;
    private const string CodeError = "";
    

    private static readonly StoreHeaderSerializer StoreHeaderSerializer = new();
    
    internal static Result<T> ReadStreamOperation<T>(string filePath, Func<BinaryReader, Result<T>> operation)
    {
        try
        {
            if (File.Exists(filePath) == false)
            {
                return Result.Failure<T>(new Error("FILE_NOT_FOUND", "The file or directory cannot be found."));
            }

            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using BinaryReader br = new(fs);
            return operation(br);
            
        }
        catch (Exception e)
        {
            return Result.Failure<T>(new Error(e.HResult.ToString(), e.Message));
        }
    }

    internal static async Task<Result<T>> ReadStreamOperationAsync<T>(string filePath, Func<BinaryReader, Task<Result<T>>> operation)
    {
        try
        {
            await using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using BinaryReader br = new(fs);
            return await operation(br);
        }
        catch (Exception e)
        {
            return Result.Failure<T>(new Error(e.HResult.ToString(), e.Message));
        }
    }

    internal static Result WriteStreamOperation(string filePath, Func<BinaryWriter, Result> operation)
    {
        try
        {
            using FileStream fs = new(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            using BinaryWriter bw = new(fs);
            return operation(bw);
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(e.HResult.ToString(), e.Message));
        }
    }
    
    internal static async Task<Result> WriteStreamOperationAsync(string filePath, Func<BinaryWriter, Task<Result>> operation)
    {
        try
        {
            await using FileStream fs = new(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, true);
            await using BinaryWriter bw = new(fs);
            return await operation(bw);
        }
        catch (Exception e)
        {
            return Result.Failure(new Error(e.HResult.ToString(), e.Message));
        }
    }

    internal static async Task<Result> CreateEmptyStoresAsync(IEnumerable<string> storePaths)
    {
        var tasks = storePaths.Select(async storePath =>
        {
            var result = await WriteStreamOperationAsync(storePath, async writer =>
            {
                await Task.CompletedTask;
                return Result.Success();
            });
            return result;
        }).ToArray();
        
        var results = await Task.WhenAll(tasks);
        
        var failures = results.Where(r => r.IsSuccess == false).ToList();
        if (failures.Count == 0) return Result.Success();
        var failedStorePaths = string.Join(", ", failures.Select(f => f));
        return Result.Failure(new Error("ERROR", failedStorePaths));
    }

    #region StoreHeader
    
    internal static Result<StoreHeader> ReadStoreHeader(string filePath)
    {
        return ReadStreamOperation(filePath, reader =>
        {
            var buffer = reader.ReadBytes(StoreHeaderSize);
            if (buffer.Length != StoreHeaderSize)
            {
                return Result.Failure<StoreHeader>(new Error(CodeError,
                    $"File header has incorrect length: {buffer.Length}."));
            }

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            
            var header = StoreHeaderSerializer.Deserialize(buffer);
            return Result.Success(header);
        });
    }

    internal static async Task<Result<StoreHeader>> ReadStoreHeaderAsync(string filePath, CancellationToken token = default)
    {
        return await ReadStreamOperationAsync(filePath, async reader =>
        {
            var buffer = new byte[StoreHeaderSize];
            await reader.BaseStream.ReadExactlyAsync(buffer, StreamOffset, StoreHeaderSize, token);
            if (buffer.Length != StoreHeaderSize)
            {
                return Result.Failure<StoreHeader>(new Error(CodeError, 
                    $"File header has incorrect length: {buffer.Length}."));
            }
            var header = StoreHeaderSerializer.Deserialize(buffer);
            return Result.Success(header);
        });
    }

    internal static Result WriteStoreHeader(string filePath, StoreHeader header)
    {
        var bytes = StoreHeaderSerializer.Serialize(header);
        var result = WriteStreamOperation(filePath, writer =>
        {
            writer.Write(bytes);
            return Result.Success();
        });
        return result;
    }

    #endregion
    
}

