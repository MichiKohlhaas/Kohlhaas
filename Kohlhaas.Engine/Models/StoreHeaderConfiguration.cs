namespace Kohlhaas.Engine.Models;

/// <summary>
/// <para>
/// 15-byte header
/// Byte 1 : store format version
/// Byte 2 : record type ID
/// Byte 3 : header file version
/// Byte 4 -5 : magic number for file validation
/// Byte 6 : record size in bytes
/// Byte 7 : Encoding (endianess), 1 for big, 0 for little
/// Byte 8 - 9 : additional parameters (unused)
/// Byte 10 : transaction log sequence number
/// Byte 11 - 12 : checksum
/// Byte 13 : last known good state
/// Byte 14 - 15 : reserved for future use
/// </para> 
/// </summary>
public class StoreHeaderConfiguration
{
    //Store format info
    public byte FormatVersion { get; init; }
    
    //Store file metadata
    public byte FileTypeId { get; init; }
    public byte FileVersion { get; init; }
    public ushort MagicNumber { get; init; }
    
    //Store file config data
    public byte RecordSize { get; init; }
    /// <summary>
    /// 1 = Big Endian
    /// 0 = Little Endian
    /// </summary>
    public byte Encoding { get; init; }
    public ushort AdditionalParameters { get; init; }
    
    //Store file recover info
    public byte TransactionLogSequence { get; init; }
    public ushort Checksum { get; init; }
    public byte Lkgs { get; init; } // Last known good state
    
    //For future use
    public ushort Reserved { get; init; }
}