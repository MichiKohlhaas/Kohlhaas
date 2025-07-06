using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Parser;

public class StoreParser : IRecordParser<StoreHeader>
{
    private const byte HeaderSize = 15;
    private const byte HeaderFileTypePos = 1;
    private const byte NodeStoreId = 1;
    private const byte LabelStoreId = 2;
    private const byte RelationshipStoreId = 3;
    private const byte PropertyStoreId = 4;

    public StoreHeader Parse(byte[] bytes) => Parse(bytes.AsSpan());
    public StoreHeader Parse(ReadOnlySpan<byte> headerData)
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
}