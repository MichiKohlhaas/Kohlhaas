namespace Kohlhaas.Engine.Stores;

// 15-byte header
public readonly struct StoreHeader
{
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
    
    //Store file additional data
    public ushort Reserved { get; init; }
}
