using System.Collections.Immutable;

namespace Kohlhaas.Common.Interfaces;

public interface INode
{
    public int Id { get; }
    /// <summary>
    /// Limited to 3 labels
    /// </summary>
    public string[]? Labels { get; }
    public List<IRelationship>? Relationships { get; }
    public ImmutableDictionary<string, object>? Properties { get; }
    
    public abstract INode AddProperty(string key, object value);
    public abstract INode UpdateSelf(INode update);
}