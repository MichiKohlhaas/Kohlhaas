using Kohlhaas.Engine.Models;
using Kohlhaas.Engine.Stores;

namespace Kohlhaas.Engine.Interfaces;

public interface IStoreHeaderFactory
{
    StoreHeader CreateStoreHeader();
    StoreHeader CreateStoreHeader(StoreHeaderConfiguration config);
}