using Kohlhaas.Domain.Entities;

namespace Kohlhaas.Domain.Interfaces;

public interface IRepositoryFactory<T> where T : IEntity
{
    T CreateRepository();
}