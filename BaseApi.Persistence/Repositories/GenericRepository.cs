using BaseApi.Abstractions.Repositories;
using BaseApi.Domain.Entities.Base;
using BaseApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.Where(predicate).ToList());
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        var result = await _dbSet.AddAsync(entity);
        return result.Entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return await Task.FromResult(entity);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public virtual async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id);
    }

    public virtual async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public virtual async Task<int> CountAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.Count(predicate));
    }

    // LINQ Extension Methods
    public virtual IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public virtual async Task<IEnumerable<T>> WhereAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.Where(predicate).ToList());
    }

    public virtual async Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.FirstOrDefault(predicate));
    }

    public virtual async Task<T?> SingleOrDefaultAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.SingleOrDefault(predicate));
    }

    public virtual async Task<IEnumerable<T>> TakeAsync(int count)
    {
        return await _dbSet.Take(count).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> SkipAsync(int count)
    {
        return await _dbSet.Skip(count).ToListAsync();
    }

    public virtual async Task<IEnumerable<T>> OrderByAsync<TKey>(Func<T, TKey> keySelector)
    {
        return await Task.FromResult(_dbSet.OrderBy(keySelector).ToList());
    }

    public virtual async Task<IEnumerable<T>> OrderByDescendingAsync<TKey>(Func<T, TKey> keySelector)
    {
        return await Task.FromResult(_dbSet.OrderByDescending(keySelector).ToList());
    }
} 