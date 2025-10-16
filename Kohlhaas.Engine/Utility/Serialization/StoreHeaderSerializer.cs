using System.Buffers.Binary;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Serialization;

public class StoreHeaderSerializer : IRecordSerializer<StoreHeader>
{
    private const byte HeaderSize = 15;
    private const byte HeaderFormatVersionPos = 0;
    private const byte HeaderFileTypePos = 1;
    private const byte HeaderFileVersionPos = 2;
    private const byte HeaderMagicNumberPos = 3;
    private const byte HeaderRecordSizePos = 5;
    private const byte HeaderEncodingPos = 6;
    private const byte HeaderAdditionalParamPos = 7;
    private const byte HeaderTransactLogSeqPos = 9;
    private const byte HeaderChecksumPos = 10;
    private const byte HeaderLkgsPos = 12;
    private const byte HeaderReservedPos = 13;
    
    private const byte NodeStoreId = 1;
    private const byte LabelStoreId = 2;
    private const byte RelationshipStoreId = 3;
    private const byte PropertyStoreId = 4;

    public StoreHeader Deserialize(byte[] bytes) => Deserialize(bytes.AsSpan());
    public StoreHeader Deserialize(ReadOnlySpan<byte> headerData)
    {
        return new StoreHeader(
            formatVersion: headerData[HeaderFormatVersionPos],
            fileTypeId: headerData[HeaderFileTypePos],
            fileVersion: headerData[HeaderFileVersionPos],
            magicNumber: BinaryPrimitives.ReadUInt16LittleEndian(headerData.Slice(HeaderMagicNumberPos, sizeof(ushort))),
            recordSize: headerData[HeaderRecordSizePos],
            encoding: headerData[HeaderEncodingPos],
            additionalParameters: BinaryPrimitives.ReadUInt16LittleEndian(headerData.Slice(HeaderAdditionalParamPos, sizeof(short))),
            transactionLogSequence: headerData[HeaderTransactLogSeqPos],
            checksum: BinaryPrimitives.ReadUInt16LittleEndian(headerData.Slice(HeaderChecksumPos, sizeof(short))),
            lkgs: headerData[HeaderLkgsPos],
            
            reserved: BinaryPrimitives.ReadUInt16LittleEndian(headerData[HeaderReservedPos..]));
    }

    public byte[] Serialize(StoreHeader header)
    {
        var data = new byte[HeaderSize];
        data[HeaderFormatVersionPos] = header.FormatVersion;
        data[HeaderFileTypePos] = header.FileTypeId;
        data[HeaderFileVersionPos] = header.FileVersion;
        BitConverter.GetBytes(header.MagicNumber).CopyTo(data, HeaderMagicNumberPos);
        data[HeaderRecordSizePos] = header.RecordSize;
        data[HeaderEncodingPos] = header.Encoding;
        BitConverter.GetBytes(header.AdditionalParameters).CopyTo(data, HeaderAdditionalParamPos);
        data[HeaderTransactLogSeqPos]  = header.TransactionLogSequence;
        BitConverter.GetBytes(header.Checksum).CopyTo(data, HeaderChecksumPos);
        data[HeaderLkgsPos] = header.Lkgs;
        BitConverter.GetBytes(header.Reserved).CopyTo(data, HeaderReservedPos);
        return data;
    }
}