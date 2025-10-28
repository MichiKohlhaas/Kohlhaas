using System.Collections.Immutable;
using Kohlhaas.Engine.Enums;

namespace Kohlhaas.Engine.Layout.RecordStorage;

public static class RecordDatabaseFileNames
{
    public const string CountsStore = "kohlhaas.countsstore.db"; //what is this?
    public const string NodesStore = "kohlhaas.nodestore.db";
    public const string NodesStoreIds = "kohlhaas.nodestore.db.id";
    public const string RelationshipStore = "kohlhaas.relationshipstore.db";
    public const string LabelsStore = "kohlhaas.nodestore.db.labels";
    public const string PropertyStore = "kohlhaas.propertystore.db";
    public const string PropertyStringStore = "kohlhaas.propertystore.db.strings";
    public const string PropertyArrayStore = "kohlhaas.propertystore.db.array";
    public const string PropertyStoreIndex = "kohlhaas.propertystore.db.index";
    public const string RelationshipGroupStore = "kohlhaas.relationshipgroupstore.db";

    public static ImmutableDictionary<StoreTypes, (string, byte)> StoreFiles { get; } = new Dictionary<StoreTypes, (string, byte)>()
    {
        {StoreTypes.CountsStoreType, (CountsStore, 1)},
        {StoreTypes.NodesStoreType, (NodesStore, 13)},
        {StoreTypes.NodesStoreIdsType, (NodesStoreIds, 4)},
        {StoreTypes.RelationshipStoreType, (RelationshipStore, 33)},
        {StoreTypes.LabelsStoreType, (LabelsStore, 65)},
        {StoreTypes.PropertyStoreType, (PropertyStore, 37)},
        {StoreTypes.PropertyStringStoreType, (PropertyStringStore, 20)},
        {StoreTypes.PropertyArrayStoreType, (PropertyArrayStore, 20)},
        {StoreTypes.PropertyStoreIndexType, (PropertyStoreIndex, 20)},
        {StoreTypes.RelationshipGroupStoreType, (RelationshipGroupStore, 20)},
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
        PropertyStoreIndex,
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
        PropertyArrayStore,
        PropertyStoreIndex
    ];
}