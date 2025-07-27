using BaseApi.Abstractions.Repositories;
using BaseApi.Domain.Entities;
using BaseApi.Domain.Enums;
using BaseApi.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BaseApi.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product> AddAsync(Product entity)
    {
        _context.Products.Add(entity);
        return entity;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        _context.Products.Update(entity);
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Products.CountAsync();
    }

    public async Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status)
    {
        return await _context.Products
            .Where(p => p.Status == status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByNameAsync(string name)
    {
        return await _context.Products
            .Where(p => p.Name.Contains(name))
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _context.Products
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10)
    {
        return await _context.Products
            .Where(p => p.Stock <= threshold)
            .ToListAsync();
    }
} 