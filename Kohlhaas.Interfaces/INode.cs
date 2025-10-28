using System.Collections.Immutable;

namespace Kohlhaas.Interfaces;

public interface INode
{
    public Guid Id { get; }
    /// <summary>
    /// Limited to 3 labels
    /// </summary>
    public string[]? Labels { get; }
    public List<IRelationship>? Relationships { get; }
    public ImmutableDictionary<string, object>? Properties { get; }
    
    public abstract INode AddProperty(string key, object value);
    public abstract INode UpdateSelf(INode update);
}