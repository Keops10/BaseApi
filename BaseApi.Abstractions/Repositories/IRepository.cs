using BaseApi.Domain.Entities;
using BaseApi.Domain.Entities.Base;

namespace BaseApi.Abstractions.Repositories;

public interface IRepository<T> where T : IBaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
} 