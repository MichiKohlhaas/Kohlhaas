using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using Kohlhaas.Engine.Stores;
using Kohlhaas.Engine.Utility.Parser;

namespace Kohlhaas.Engine.Utility.IO;

internal static class DataAccess
{
    private const byte StoreHeaderSize = 15;
    private const byte MsrSize = 92;
    private const byte StreamOffset = 0;
    private const string CodeError = "";
    private const string MsrFile = "Kohlhaas.MSR.db";

    private static readonly StoreHeaderParser StoreHeaderParser = new();
    
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
    
    //write stream op'n async()

    internal static Result<StoreHeader> ReadStoreHeader(string filePath)
    {
        return ReadStreamOperation(filePath, reader =>
        {
            ReadOnlySpan<byte> buffer = reader.ReadBytes(StoreHeaderSize);
            if (buffer.Length != StoreHeaderSize)
            {
                return Result.Failure<StoreHeader>(new Error(CodeError,
                    $"File header has incorrect length: {buffer.Length}."));
            }
            var header = StoreHeaderParser.ParseTo(buffer);
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
            var header = StoreHeaderParser.ParseTo(buffer);
            return Result.Success(header);
        });
    }

    internal static Result WriteStoreHeader(string filePath, StoreHeader header)
    {
        var bytes = StoreHeaderParser.ParseFrom(header);
        var result = WriteStreamOperation(filePath, writer =>
        {
            writer.Write(bytes);
            return Result.Success();
        });
        return result;
    }

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
            return ReadStreamOperation(msrFilePath, reader =>
            {
                var msrData = reader.ReadBytes(MsrSize);
                var msr = new MasterStoreRecord(subDirectories.ToList(), msrData);
                return Result.Success(msr);
            });
        }
        catch (Exception e)
        {
            return Result.Failure<MasterStoreRecord>(new Error(e.HResult.ToString(), e.Message));
        }
    }
}

