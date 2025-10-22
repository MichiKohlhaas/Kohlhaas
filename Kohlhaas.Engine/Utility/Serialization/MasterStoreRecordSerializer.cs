using System.Text;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Serialization;

public class MasterStoreRecordSerializer
{
    private const char CollectionSeparator = '|';
    private const byte MsrSize = 92;
    public byte[] Serialize(MasterStoreRecord msr)
    {
        var data = new List<byte>();
        data.AddRange(msr.Data);
        foreach (var temp in msr.Collections.Select(col => Encoding.UTF8.GetBytes(col)))
        {
            data.AddRange(temp);
            
            data.Add(Convert.ToByte(CollectionSeparator));
        }
        return data.ToArray();
    }

    public MasterStoreRecord Deserialize(byte[] data) => Deserialize(data.AsSpan());

    private MasterStoreRecord Deserialize(ReadOnlySpan<byte> msrData)
    {
        if (msrData.Length < MsrSize)
        {
            throw new ArgumentException($"MasterStoreRecord length is too short. Must be at least {MsrSize} bytes long", nameof(msrData));
        }
        var data = msrData[..MsrSize].ToArray();
        var remainingBytes = msrData[MsrSize..];
        var collection = remainingBytes.Length > 0 
            ? Encoding.UTF8.GetString(remainingBytes.ToArray()).Split(CollectionSeparator, StringSplitOptions.RemoveEmptyEntries).ToList() 
            : [];
        return new MasterStoreRecord(collection, data);
    }
}