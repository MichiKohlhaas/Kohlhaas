using System.Collections.Immutable;

namespace Kohlhaas.Interfaces;

public interface INode
{
    public Guid Id { get; }
    public string VLevel { get; }
    public ImmutableDictionary<string, object> Properties { get; }
    public string FileTag { get; }
    public string FileName { get; }
    public string UniqueFileName { get; }
    
    public abstract INode AddProperty(string key, object value);
    public abstract INode UpdateSelf(INode update);
}