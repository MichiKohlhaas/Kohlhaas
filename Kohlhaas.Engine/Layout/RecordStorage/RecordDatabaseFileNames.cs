namespace Kohlhaas.Engine.Layout.RecordStorage;

public static class RecordDatabaseFileNames
{
    private const string CountsStore = "kohlhaas.countsstore.db";
    private const string NodesStore = "kohlhaas.nodesstore.db";
    private const string RelationshipStore = "kohlhaas.relationshipstore.db";
    private const string LabelsStore = "kohlhaas.nodestore.db.labels";
    private const string PropertyStore = "kohlhaas.propertystore.db";
    private const string PropertyStringStore = "kohlhaas.propertystore.db.strings";
    private const string PropertyArrayStore = "kohlhaas.propertystore.db.array";
    private const string RelationshipGroupStore = "kohlhaas.relationshipgroupstore.db";

    public static readonly List<string> FileNames = [
        CountsStore,
        NodesStore,
        RelationshipStore,
        LabelsStore,
        PropertyStore,
        PropertyStringStore,
        PropertyArrayStore,
        RelationshipGroupStore
    ];
}