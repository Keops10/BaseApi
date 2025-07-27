using BaseApi.Domain.Entities.Base;

namespace BaseApi.Abstractions.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IUserRepository Users { get; }
    IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity;
    Task<int> CommitAsync();
} 