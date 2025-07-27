using BaseApi.Domain.Entities.Base;

namespace BaseApi.Abstractions.Repositories;

public interface IGenericRepository<T> where T : class, IBaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();
    Task<int> CountAsync(Func<T, bool> predicate);
    
    // LINQ Extension Methods
    IQueryable<T> Query();
    Task<IEnumerable<T>> WhereAsync(Func<T, bool> predicate);
    Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate);
    Task<T?> SingleOrDefaultAsync(Func<T, bool> predicate);
    Task<IEnumerable<T>> TakeAsync(int count);
    Task<IEnumerable<T>> SkipAsync(int count);
    Task<IEnumerable<T>> OrderByAsync<TKey>(Func<T, TKey> keySelector);
    Task<IEnumerable<T>> OrderByDescendingAsync<TKey>(Func<T, TKey> keySelector);
} 