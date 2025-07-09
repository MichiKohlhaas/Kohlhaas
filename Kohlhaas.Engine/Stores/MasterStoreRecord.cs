namespace Kohlhaas.Engine.Stores;

/// <summary>
/// Master file that sits at the top of the Kohlhaas.Engine directory. Contains metadata about the state of the database
/// and a list of the Collections (tables). 
/// </summary>
public record MasterStoreRecord
{
    public List<string> Collections { get; set; }
    /// <summary>
    /// 92 bytes of metadata. No use yet.
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Ctor for working with existing data.
    /// </summary>
    /// <param name="collections">List of Collections in the database directory</param>
    /// <param name="data">Bytes read from the file</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if file is not 92 bytes.</exception>
    public MasterStoreRecord(List<string> collections, byte[] data)
    {
        Collections = collections;
        if (data.Length != 92) throw new ArgumentOutOfRangeException(nameof(data), "Data length is not 92");
        Data = data;
    }

    /// <summary>
    /// Empty ctor for first startup purposes.
    /// </summary>
    public MasterStoreRecord()
    {
        Collections = [];
        Data = [];
    }
}