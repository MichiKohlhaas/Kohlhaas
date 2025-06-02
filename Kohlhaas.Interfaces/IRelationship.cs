namespace Kohlhaas.Interfaces;

public interface IRelationship
{
    public Guid Id { get; set; }
    public INode StartNode { get; set; }
    public INode EndNode { get; set; }
    public RelationshipType Type { get; set; }
    public IDictionary<string, object> Properties { get; set; }
}