using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Text;
using Kohlhaas.Engine.Stores;
using Kohlhaas.Engine.Utility.Serialization;

namespace Kohlhaas.Engine.Utility.IO;

internal static class DataAccess
{
    private const byte StoreHeaderSize = 15;
    private const byte StreamOffset = 0;
    private const string CodeError = "";
    

    private static readonly StoreHeaderSerializer StoreHeaderSerializer = new();
    private static readonly LabelSerializer LabelSerializer = new();
    //private static readonly PropertyBlockSerializer PropertyBlockSerializer = new();
    private static readonly PropertySerializer PropertySerializer = new();
    
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

    internal static async Task<Result> WriteStoreHeaderAsync(string filePath, StoreHeader header,
        CancellationToken token = default)
    {
        var bytes = StoreHeaderSerializer.Serialize(header);
        var task = await WriteStreamOperationAsync(filePath, async writer =>
        {
            writer.Write(bytes);
            await Task.CompletedTask;
            return Result.Success();
        });
        return task;
    }

    #endregion
    
    #region Label

    internal static Result<LabelRecord> CreateLabelRecord(string filePath, byte[] labelData)
    {
        if (labelData.Length > 60) return Result.Failure<LabelRecord>(new Error("Error code", "Label data is too long."));
        var storeHeader = ReadStoreHeader(filePath);
        if (storeHeader.IsSuccess is false) return Result.Failure<LabelRecord>(storeHeader.Error);
            
        var labelRecord = new LabelRecord()
        {
            InUse = 1,
            ReservedSpace = 0,
            LabelData = labelData
        };
        //Todo: check labels.db.id for available IDs and seek to there
        var seekPosition = 0;
        var serializedLabel = LabelSerializer.Serialize(labelRecord);
        var labelResult = WriteStreamOperation(filePath, writer =>
            {
                //write labels
                writer.Seek(seekPosition, SeekOrigin.End);
                writer.Write(serializedLabel);
                return Result.Success();
            });
        //if (labelResult.IsSuccess is false) return Result.Failure<LabelRecord>(labelResult.Error);
        
        //Todo: update transaction log file
        
        /*StoreHeader sh = new StoreHeader(
            formatVersion: storeHeader.Value.FormatVersion,
            fileTypeId: storeHeader.Value.FileTypeId,
            fileVersion: storeHeader.Value.FileVersion,
            magicNumber: storeHeader.Value.MagicNumber,
            recordSize: storeHeader.Value.RecordSize,
            encoding: storeHeader.Value.Encoding,
            additionalParameters: storeHeader.Value.AdditionalParameters,
            transactionLogSequence: (byte)(storeHeader.Value.TransactionLogSequence + 1),
            checksum: storeHeader.Value.Checksum,
            lkgs: storeHeader.Value.Lkgs,
            reserved: storeHeader.Value.Reserved
        );
        var serializedStoreHeader = StoreHeaderSerializer.Serialize(sh);*/
        
        return labelResult.IsSuccess ? Result.Success(labelRecord) : Result.Failure<LabelRecord>(labelResult.Error);
    }
    
    
    internal static Result<LabelRecord> ReadLabelRecord(string filePath)
    {
        throw new NotImplementedException();
    }
    
    
    #endregion
    
    #region Property

    internal static Result<PropertyRecord> CreatePropertyRecord(string filePath, IDictionary<string, object> properties)
    {
        if(properties.Count > 4) return Result.Failure<PropertyRecord>(new Error("Error code", "Property data is too long."));
        var storeHeader = ReadStoreHeader(filePath);
        if (storeHeader.IsSuccess is false) return Result.Failure<PropertyRecord>(storeHeader.Error);
        
        var propertyBlocks = CreatePropertyBlocks(properties);

        var propertyRecord = new PropertyRecord()
        {
            InUse = 1,
            NextPropId = 0,
            PrevPropId = 0,
            PropertyBlocks = propertyBlocks,
            
        };
        var seekPosition = 0;
        var serializedLabel = PropertySerializer.Serialize(propertyRecord);
        var propertyResult = WriteStreamOperation(filePath, writer =>
        {
            //write labels
            writer.Seek(seekPosition, SeekOrigin.End);
            writer.Write(serializedLabel);
            return Result.Success();
        });
        
        return propertyResult.IsSuccess ? Result.Success(propertyRecord) : Result.Failure<PropertyRecord>(propertyResult.Error);
    }

    private static PropertyBlock[] CreatePropertyBlocks(IDictionary<string, object> properties)
    {
        var propertyBlocks = new PropertyBlock[4];
        var counter = 0;
        foreach (var property in properties)
        {
            //will only work if value is 8 characters or fewer
            var bytes = Encoding.Unicode.GetBytes((string)property.Value);
            var bytesToUse = Math.Min(bytes.Length, sizeof(ulong));
            ulong value = 0;
            for (var i = 0; i < bytesToUse; i += 2)
            {
                value = (value << 8) | bytes[i];
            }
            propertyBlocks[counter] = new PropertyBlock()
            {
                Key = 0x01,
                PropertyType = (uint)(properties.Values is string ? 0 : 1),//0 for string, 1 for array
                Value = value,
            };
            counter++;
        }

        return propertyBlocks;
    }

    #endregion
}

