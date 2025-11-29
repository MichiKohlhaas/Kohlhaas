using System.Buffers.Binary;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Text;
using Kohlhaas.Engine.Layout.RecordStorage;
using Kohlhaas.Common.Models;
using Kohlhaas.Common.Interfaces;
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
    private static readonly NodeRecordSerializer NodeRecordSerializer = new();
    
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

    #region Relationship

    public static Result<RelationshipRecord> CreateRelationshipRecord(string filePath, IRelationship relationship)
    {
        return Result.Success<RelationshipRecord>(new RelationshipRecord());
        var relationshipRecord = new RelationshipRecord()
        {
            InUse = 1,
            FirstNode = 0,
            SecondNode = 0,
            RelationshipType = (byte)relationship.Type,
            FirstPrevRelId = 0,
            FirstNextRelId = 0,
            SecondPrevRelId = 0,
            SecondNextRelId = 0,
            NextPropId = 0
        };
    }

    #endregion
    
    #region Label

    internal static Result<(LabelRecord record, long id)> CreateLabelRecord(string filePath, byte[] labelData)
    {
        if (labelData.Length > 60) return Result.Failure<(LabelRecord, long)>(new Error("Error code", "Label data is too long."));
        var storeHeader = ReadStoreHeader(filePath);
        if (storeHeader.IsSuccess is false) return Result.Failure<(LabelRecord, long)>(storeHeader.Error);
        
        var fileSize = Math.Max(GetFileSize(filePath) - StoreHeaderSize, 0);
        var labelCount = fileSize / storeHeader.Value.RecordSize;
        var labelId = labelCount + 1;
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
        
        return labelResult.IsSuccess ? Result.Success((labelRecord, labelId)) : Result.Failure<(LabelRecord, long)>(labelResult.Error);
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

        // Could be an issue if filePath returns -1, maybe should add a check
        var fileSize = Math.Max(GetFileSize(filePath) - StoreHeaderSize, 0);
        // if property count = 0, then should arbitrarily make the first ID = 1, 0 indicates no previous props, first prop in file
        var propertyCount = fileSize / storeHeader.Value.RecordSize;
        var previousPropertyId =(ushort)propertyCount;
        var nextPropertyId = (ushort)(previousPropertyId + 2);
        
        var propertyBlocks = CreatePropertyBlocks(properties);

        var propertyRecord = new PropertyRecord()
        {
            InUse = 1,
            NextPropId = nextPropertyId,
            PrevPropId = previousPropertyId,
            PropertyBlocks = propertyBlocks,
            
        };
        var seekPosition = 0;
        var serializedProperty = PropertySerializer.Serialize(propertyRecord);
        var propertyResult = WriteStreamOperation(filePath, writer =>
        {
            writer.Seek(seekPosition, SeekOrigin.End);
            writer.Write(serializedProperty);
            return Result.Success();
        });
        
        return propertyResult.IsSuccess ? Result.Success(propertyRecord) : Result.Failure<PropertyRecord>(propertyResult.Error);
    }

    private static PropertyBlock[] CreatePropertyBlocks(IDictionary<string, object> properties)
    {
        var propertyBlocks = new PropertyBlock[4];
        var propertyIndices = new PropertyIndexRecord[4];
        var counter = 0;
        foreach (var property in properties)
        {
            //will only work if value is 8 characters or fewer
            var bytes = Encoding.UTF8.GetBytes((string)property.Value);
            var bytesToUse = Math.Min(bytes.Length, sizeof(ulong));
            ulong value = 0;
            for (var i = 0; i < bytesToUse; i += 2)
            {
                value = (value << 8) | bytes[i];
            }
            propertyBlocks[counter] = new PropertyBlock()
            {
                Key = (byte)(0x00 + counter),
                PropertyType = (uint)(properties.Values is string ? 0 : 1),//0 for string, 1 for array
                Value = value,
            };
            /*propertyIndices[counter] = new PropertyIndexRecord()
            {
                InUse = 1,
                //RoN = (byte)(true ? 1 : 0),
                //id = Relationship || Node.Id <-- get this only after writing the node to the file, so when to write prop index?
                RecordLength = (byte)bytes.Length,
                Key = propertyBlocks[counter].Key,
                Name = BinaryPrimitives.ReadUInt64LittleEndian(Encoding.UTF8.GetBytes(property.Key)),
            };*/
            counter++;
        }

        return propertyBlocks;
    }

    #endregion

    private static long GetFileSize(string filePath)
    {
        try
        {
            FileInfo fileInfo = new(filePath);
            return fileInfo.Length;
        }
        catch (Exception e)
        {
            return -1;
        }
    }

    #region Node
    public static Result<(NodeRecord record, long nodeId)> CreateNodeRecord(string path, uint labelsId, Result<PropertyRecord>? propertyRecord)
    {
        var storeHeader = ReadStoreHeader(path);
        if (storeHeader.IsSuccess is false) return Result.Failure<(NodeRecord, long)>(storeHeader.Error);
        
        var fileSize = Math.Max(GetFileSize(path) - StoreHeaderSize, 0);
        var nodeCount = fileSize / storeHeader.Value.RecordSize;
        var nodeId = nodeCount + 1;
        
        var nodeRecord = new NodeRecord()
        {
            InUse = 1,
            Labels = labelsId,
            NextPropId = propertyRecord is {IsSuccess: true} ? (uint)(propertyRecord.Value.NextPropId - 1) : 0
        };
        var serializedNode = NodeRecordSerializer.Serialize(nodeRecord);
        var nodeResult = WriteStreamOperation(path, writer =>
        {
            writer.Seek(0, SeekOrigin.End);
            writer.Write(serializedNode);
            return Result.Success();
        });
        return nodeResult.IsSuccess ? Result.Success((nodeRecord, nodeId)) : Result.Failure<(NodeRecord, long)>(nodeResult.Error);
    }

    

    #endregion

    #region Relationship
    public static Result<RelationshipRecord>? ReadRelationshipRecord(IRelationship relationship)
    {
        throw new NotImplementedException();
    }
    #endregion
}

