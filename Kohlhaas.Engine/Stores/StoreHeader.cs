using System.Buffers.Binary;

namespace Kohlhaas.Engine.Stores;

/// <summary>
/// <para>
/// 15-byte header
/// Byte 1 : store format version
/// Byte 2 : record type ID
/// Byte 3 : header file version
/// Byte 4 -5 : magic number for file validation
/// Byte 6 : record size in bytes
/// Byte 7 : Encoding (endianess)
/// Byte 8 - 9 : additional parameters (unused)
/// Byte 10 : transaction log sequence number
/// Byte 11 - 12 : checksum
/// Byte 13 : last known good state
/// Byte 14 - 15 : reserved for future use
/// </para> 
/// </summary>
public readonly struct StoreHeader
{
    private const byte HeaderSize = 15;
    public StoreHeader(byte formatVersion, byte fileTypeId, byte fileVersion, ushort magicNumber, byte recordSize, byte encoding, ushort additionalParameters, byte transactionLogSequence, ushort checksum, byte lkgs)
    {
        FormatVersion = formatVersion;
        FileTypeId = fileTypeId;
        FileVersion = fileVersion;
        MagicNumber = magicNumber;
        RecordSize = recordSize;
        Encoding = encoding;
        AdditionalParameters = additionalParameters;
        TransactionLogSequence = transactionLogSequence;
        Checksum = checksum;
        Lkgs = lkgs;
    }

    public StoreHeader(byte[] bytes)
    {
        if (bytes.Length != HeaderSize) throw new ArgumentException($"Byte array is not {HeaderSize}-bytes in length");
        
        FormatVersion = bytes[0];
        FileTypeId = bytes[1];
        FileVersion = bytes[2];
        RecordSize = bytes[5];
        Encoding = bytes[6];
        TransactionLogSequence = bytes[9];
        Lkgs = bytes[12];
        if (BitConverter.IsLittleEndian)
        {
            MagicNumber = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan()[3..5]);
            AdditionalParameters = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan()[7..9]);
            Checksum = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan()[10..12]);
            Reserved = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan()[13..15]);
            
        }
        else
        {
            MagicNumber = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan()[3..5]);
            AdditionalParameters = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan()[7..9]);
            Checksum = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan()[10..12]);
            Reserved = BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan()[13..15]);
        }
    }
    
    //Store format info
    public byte FormatVersion { get; init; }
    
    //Store file metadata
    public byte FileTypeId { get; init; }
    public byte FileVersion { get; init; }
    public ushort MagicNumber { get; init; }
    
    //Store file config data
    public byte RecordSize { get; init; }
    public byte Encoding { get; init; }
    public ushort AdditionalParameters { get; init; }
    
    //Store file recover info
    public byte TransactionLogSequence { get; init; }
    public ushort Checksum { get; init; }
    public byte Lkgs { get; init; } // Last known good state
    
    //For future use
    public ushort Reserved { get; init; }
}
