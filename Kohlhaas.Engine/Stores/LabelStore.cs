namespace Kohlhaas.Engine.Stores;

// May be used.
public readonly struct LabelStore(StoreHeader header, LabelRecord[] labels)
{
    public StoreHeader Header { get; init; } = header;
    public LabelRecord[] Labels { get; init; } = labels;
}