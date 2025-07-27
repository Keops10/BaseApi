using BaseApi.Domain.Entities;
using BaseApi.Domain.Enums;

namespace BaseApi.Abstractions.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByStatusAsync(ProductStatus status);
    Task<IEnumerable<Product>> GetByNameAsync(string name);
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold = 10);
} 