using System.IO.MemoryMappedFiles;
using Kohlhaas.Engine.Stores;
using Kohlhaas.Engine.Utility.Parser;

namespace Kohlhaas.Engine.Utility.IO;

public static class DataAccess
{
    private const byte StoreHeaderSize = 15;
    private const string CODE_ERROR = "";
    public static Result<T> ReadStreamOperation<T>(string filePath, Func<BinaryReader, Result<T>> operation)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return Result.Failure<T>(new Error("FILE_NOT_FOUND", "The file or directory cannot be found."));
            }

            using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using BinaryReader br = new(fs);
            return operation(br);
            
        }
        catch (Exception e)
        {
            return Result.Failure<T>(new Error(e.HResult.ToString(), e.Message));
        }
    }

    public static async Task<Result<T>> ReadStreamOperationAsync<T>(string filePath, Func<BinaryReader, Task<T>> operation)
    {
        try
        {
            await using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using BinaryReader br = new(fs);
            return await operation(br);
        }
        catch (Exception e)
        {
            return Result.Failure<T>(new Error(e.HResult.ToString(), e.Message));
        }
    }

    public static Result<T> WriteStreamOperation<T>(string filePath, Func<BinaryWriter, Result<T>> operation)
    {
        try
        {
            using FileStream fs = new(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            using BinaryWriter bw = new(fs);
            return operation(bw);
        }
        catch (Exception e)
        {
            return Result.Failure<T>(new Error(e.HResult.ToString(), e.Message));
        }
    }

    public static Result<StoreHeader> ReadStoreHeader(string filePath)
    {
        return ReadStreamOperation(filePath, reader =>
        {
            try
            {
                var bytes = reader.ReadBytes(StoreHeaderSize);
                if (bytes.Length != StoreHeaderSize)
                    return Result.Failure<StoreHeader>(new Error($"{CODE_ERROR}", "File header has incorrect length."));
                var header = new StoreHeader(bytes);
                return Result.Success(header);
            }
            catch (Exception e)
            {
                return Result.Failure<StoreHeader>(new Error(e.HResult.ToString(), e.Message));
            }
        });
    }
}