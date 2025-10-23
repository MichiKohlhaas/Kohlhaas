using System.Collections.Immutable;
using Kohlhaas.Engine.Enums;

namespace Kohlhaas.Engine.Layout.RecordStorage;

public static class RecordDatabaseFileNames
{
    public const string CountsStore = "kohlhaas.countsstore.db";
    public const string NodesStore = "kohlhaas.nodestore.db";
    public const string NodesStoreIds = "kohlhaas.nodestore.db.id";
    public const string RelationshipStore = "kohlhaas.relationshipstore.db";
    public const string LabelsStore = "kohlhaas.nodestore.db.labels";
    public const string PropertyStore = "kohlhaas.propertystore.db";
    public const string PropertyStringStore = "kohlhaas.propertystore.db.strings";
    public const string PropertyArrayStore = "kohlhaas.propertystore.db.array";
    public const string RelationshipGroupStore = "kohlhaas.relationshipgroupstore.db";

    public static ImmutableDictionary<StoreTypes, string> StoreFiles { get; } = new Dictionary<StoreTypes, string>()
    {
        {StoreTypes.CountsStoreType, CountsStore},
        {StoreTypes.NodesStoreType, NodesStore},
        {StoreTypes.NodesStoreIdsType, NodesStoreIds},
        {StoreTypes.RelationshipStoreType, RelationshipStore},
        {StoreTypes.LabelsStoreType, LabelsStore},
        {StoreTypes.PropertyStoreType, PropertyStore},
        {StoreTypes.PropertyStringStoreType, PropertyStringStore},
        {StoreTypes.PropertyArrayStoreType, PropertyArrayStore},
        {StoreTypes.RelationshipGroupStoreType, RelationshipGroupStore},
    }.ToImmutableDictionary();
    
    public static readonly List<string> FileNames = [
        CountsStore,
        NodesStore,
        NodesStoreIds,
        RelationshipStore,
        LabelsStore,
        PropertyStore,
        PropertyStringStore,
        PropertyArrayStore,
        RelationshipGroupStore
    ];

    public static readonly List<string> NodeStoreFiles = [
        NodesStore,
        NodesStoreIds,
        LabelsStore
    ];
    
    public static readonly List<string> PropertyStoreFiles = [
        PropertyStore,
        PropertyStringStore,
        PropertyArrayStore
    ];
}