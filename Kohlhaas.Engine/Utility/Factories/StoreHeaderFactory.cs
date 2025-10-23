using Kohlhaas.Engine.Interfaces;
using Kohlhaas.Engine.Models;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Utility.Factories;

public class StoreHeaderFactory : IStoreHeaderFactory
{
    public StoreHeader CreateStoreHeader()
    {
        return new StoreHeader();
    }

    public StoreHeader CreateStoreHeader(StoreHeaderConfiguration config)
    {
        // Could add logic checks here
        // FileTypeID corresponds to the enum StoreTypes
        return new StoreHeader(config.FormatVersion, config.FileTypeId, config.FileVersion, config.MagicNumber, config.RecordSize, config.Encoding, config.AdditionalParameters, config.TransactionLogSequence, config.Checksum, config.Lkgs, config.Reserved);
    }
}