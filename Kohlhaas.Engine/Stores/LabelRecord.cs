using System.Text;

namespace Kohlhaas.Engine.Stores;

/// <summary>
/// Byte 1: [inUse, 0b0 or 0b1]
/// Byte 2 - 21: [:Label1]
/// Byte 22 - 41: [:Label2]
/// Byte 42 - 61: [:Label3]
/// Nodes/Relationships can have 3 labels, set aside in blocks of 20 bytes each.
/// </summary>
public readonly struct LabelRecord
{
    //Calculated from offset in file on disk, not to be written to disk
    private readonly uint _id;
    private const byte BlockSize = 20;
    public byte InUse { get; }
    public byte[] Label1 { get; }
    public byte[] Label2 { get; }
    public byte[] Label3 { get; }

    public LabelRecord(string label1, byte inUse, uint id)
    {
        _id = id;
        InUse = inUse;
        Label1 = new byte[BlockSize];
        Label2 = new byte[BlockSize];
        Label3 = new byte[BlockSize];
        
        var bytesLabel1 = Encoding.UTF8.GetBytes(label1);
        if (bytesLabel1.Length > BlockSize)
        {
            throw new Exception("Label exceeds 20 characters");
        }
        Buffer.BlockCopy(bytesLabel1, 0, Label1,0, bytesLabel1.Length);
    }
    
    public LabelRecord(string label1, string label2, byte inUse, uint id)
    {
        _id = id;
        InUse = inUse;
        Label1 = new byte[BlockSize];
        Label2 = new byte[BlockSize];
        Label3 = new byte[BlockSize];
        
        var bytesLabel1 = Encoding.UTF8.GetBytes(label1);
        var bytesLabel2 = Encoding.UTF8.GetBytes(label2);
        
        if (bytesLabel1.Length + bytesLabel2.Length > BlockSize * 2)
        {
            throw new Exception("One or more labels exceed 20 characters");
        }
        
        Buffer.BlockCopy(bytesLabel1, 0, Label1,0, bytesLabel1.Length);
        Buffer.BlockCopy(bytesLabel2, 0, Label2,0, bytesLabel2.Length);
    }
    
    public LabelRecord(string label1, string label2, string label3, byte inUse, uint id)
    {
        _id = id;
        InUse = inUse;
        Label1 = new byte[BlockSize];
        Label2 = new byte[BlockSize];
        Label3 = new byte[BlockSize];
        
        var bytesLabel1 = Encoding.UTF8.GetBytes(label1);
        var bytesLabel2 = Encoding.UTF8.GetBytes(label2);
        var bytesLabel3 = Encoding.UTF8.GetBytes(label3);
        
        if (bytesLabel1.Length + bytesLabel2.Length + bytesLabel3.Length > BlockSize * 3)
        {
            throw new Exception("One or more labels exceed 20 characters");
        }
        
        Buffer.BlockCopy(bytesLabel1, 0, Label1,0, bytesLabel1.Length);
        Buffer.BlockCopy(bytesLabel2, 0, Label2,0, bytesLabel2.Length);
        Buffer.BlockCopy(bytesLabel3, 0, Label3,0, bytesLabel3.Length);
    }

    public bool Equals(LabelRecord other)
    {
        return this.Label1.SequenceEqual(other.Label1) 
               && this.Label2.SequenceEqual(other.Label2) 
               && this.Label3.SequenceEqual(other.Label3);
    }

    // I don't plan to use this...
    public override int GetHashCode()
    {
        return 1;
    }
}