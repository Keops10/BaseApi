using BaseApi.Abstractions.Repositories;
using BaseApi.Domain.Entities.Base;
using BaseApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly Dictionary<Type, object> _repositories;
    private bool _disposed = false;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
    }

    public IProductRepository Products => GetRepository<IProductRepository, ProductRepository>();
    public IUserRepository Users => GetRepository<IUserRepository, UserRepository>();

    public IGenericRepository<T> GetRepository<T>() where T : class, IBaseEntity
    {
        var type = typeof(T);
        
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = new GenericRepository<T>(_context);
        }

        return (IGenericRepository<T>)_repositories[type];
    }

    private TInterface GetRepository<TInterface, TImplementation>() where TInterface : class where TImplementation : class, TInterface
    {
        var type = typeof(TInterface);
        
        if (!_repositories.ContainsKey(type))
        {
            _repositories[type] = Activator.CreateInstance(typeof(TImplementation), _context)!;
        }

        return (TInterface)_repositories[type];
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
} 