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

    private const byte CountStoreRecordSize = 1;
    private const byte NodesStoreRecordSize = 13;
    private const byte NodesStoreIdsSize = 4;
    private const byte RelationshipStoreRecordSize = 30;
    private const byte LabelsStoreRecordSize = 65;
    private const byte PropertyStoreRecordSize = 37;
    private const byte PropertyStringStoreRecordSize = 0;
    private const byte PropertyArrayStoreRecordSize = 0;
    private const byte PropertyStoreIndexSize = 16;

    public static ImmutableDictionary<StoreTypes, (string, byte)> StoreFiles { get; } = new Dictionary<StoreTypes, (string, byte)>()
    {
        {StoreTypes.CountsStoreType, (CountsStore, CountStoreRecordSize)},
        {StoreTypes.NodesStoreType, (NodesStore, NodesStoreRecordSize)},
        {StoreTypes.NodesStoreIdsType, (NodesStoreIds, NodesStoreIdsSize)},
        {StoreTypes.RelationshipStoreType, (RelationshipStore, RelationshipStoreRecordSize)},
        {StoreTypes.LabelsStoreType, (LabelsStore, LabelsStoreRecordSize)},
        {StoreTypes.PropertyStoreType, (PropertyStore, PropertyStoreRecordSize)},
        {StoreTypes.PropertyStringStoreType, (PropertyStringStore, PropertyStringStoreRecordSize)},
        {StoreTypes.PropertyArrayStoreType, (PropertyArrayStore, PropertyArrayStoreRecordSize)},
        {StoreTypes.PropertyStoreIndexType, (PropertyStoreIndex, PropertyStoreIndexSize)},
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