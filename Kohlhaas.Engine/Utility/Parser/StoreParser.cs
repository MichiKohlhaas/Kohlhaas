using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public static class StoreParser<T> where T : struct
{
    private const byte HeaderSize = 15;
    private const byte HeaderFileTypePos = 1;
    private const byte NodeStoreId = 1;
    private const byte LabelStoreId = 2;
    private const byte RelationshipStoreId = 3;
    private const byte PropertyStoreId = 4;
    
    public static T Parse<T>(BinaryReader reader)
    {
        T value =  default;
        return value;
    }

    public static StoreHeader ParseHeader(BinaryReader reader)
    {
        return new StoreHeader(
            formatVersion: reader.ReadByte(),
            fileTypeId: reader.ReadByte(),
            fileVersion: reader.ReadByte(),
            magicNumber: reader.ReadUInt16(),
            recordSize: reader.ReadByte(),
            encoding: reader.ReadByte(),
            additionalParameters: reader.ReadUInt16(),
            transactionLogSequence: reader.ReadByte(),
            checksum: reader.ReadUInt16(),
            lkgs: reader.ReadByte());
    }

    public static StoreHeader ParseHeader(ReadOnlySpan<byte> headerData)
    {
        return new StoreHeader(
            formatVersion: headerData[0],
            fileTypeId: headerData[1],
            fileVersion: headerData[2],
            magicNumber: BinaryPrimitives.ReadUInt16LittleEndian(headerData.Slice(3, 2)),
            recordSize: headerData[5],
            encoding: headerData[6],
            additionalParameters: BinaryPrimitives.ReadUInt16LittleEndian(headerData.Slice(7, 2)),
            transactionLogSequence: headerData[9],
            checksum: BinaryPrimitives.ReadUInt16LittleEndian(headerData.Slice(10, 2)),
            lkgs: headerData[12]);
    }

    public static void ChooseParser(byte[] headerData)
    {
        switch (headerData[HeaderFileTypePos])
        {
            case NodeStoreId:
                break;
            case LabelStoreId:
                break;
            case RelationshipStoreId:
                break;
            case PropertyStoreId:
                break;
        }
    }
}