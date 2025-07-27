using BaseApi.Abstractions.Results;

namespace BaseApi.Abstractions.Services;

public interface IProductService
{
    Task<IResult> CreateProductAsync(object createProductDto);
    Task<IResult> GetProductByIdAsync(Guid id);
    Task<IResult> GetAllProductsAsync();
    Task<IResult> UpdateProductAsync(Guid id, object updateProductDto);
    Task<IResult> DeleteProductAsync(Guid id);
    Task<IResult> RestoreProductAsync(Guid id);
    Task<IResult> UpdateStockAsync(Guid id, int newStock);
    Task<IResult> GetLowStockProductsAsync(int threshold = 10);
} 