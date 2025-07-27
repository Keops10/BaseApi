using BaseApi.Abstractions.Repositories;
using BaseApi.Abstractions.Results;
using BaseApi.Abstractions.Services;
using BaseApi.Application.Dtos;
using BaseApi.Domain.Entities;
using BaseApi.Domain.ValueObjects;
using AutoMapper;

namespace BaseApi.Application.Services;

public class ProductService : IProductService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> CreateProductAsync(object createProductDtoObj)
    {
        try
        {
            var createProductDto = (CreateProductDto)createProductDtoObj;
            
            // Debug: DTO değerlerini kontrol et
            Console.WriteLine($"DTO Values: Name={createProductDto.Name}, Description={createProductDto.Description}, Price={createProductDto.Price}, Currency={createProductDto.Currency}, Stock={createProductDto.Stock}");
            
            var product = _mapper.Map<Product>(createProductDto);

            var createdProduct = await _unitOfWork.GetRepository<Product>().AddAsync(product);
            await _unitOfWork.CommitAsync();

            var productDto = _mapper.Map<ProductDto>(createdProduct);
            return new SuccessResult<ProductDto>(productDto, "Ürün başarıyla oluşturuldu");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ProductService CreateProductAsync Error: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return new ErrorResult($"Ürün oluşturulurken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> GetProductByIdAsync(Guid id)
    {
        try
        {
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return new NotFoundResult<ProductDto>("Ürün bulunamadı");
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return new SuccessResult<ProductDto>(productDto);
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ürün getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> GetAllProductsAsync()
    {
        try
        {
            var products = await _unitOfWork.GetRepository<Product>().GetAllAsync();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return new SuccessResult<IEnumerable<ProductDto>>(productDtos);
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ürünler getirilirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> UpdateProductAsync(Guid id, object updateProductDtoObj)
    {
        try
        {
            var updateProductDto = (UpdateProductDto)updateProductDtoObj;
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return new NotFoundResult<ProductDto>("Ürün bulunamadı");
            }

            // AutoMapper ile güncelle
            _mapper.Map(updateProductDto, product);
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = "system";

            var updatedProduct = await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            await _unitOfWork.CommitAsync();

            var productDto = _mapper.Map<ProductDto>(updatedProduct);
            return new SuccessResult<ProductDto>(productDto, "Ürün güncellendi");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ürün güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> DeleteProductAsync(Guid id)
    {
        try
        {
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return new NotFoundResult("Ürün bulunamadı");
            }

            // Soft delete
            product.SoftDelete("system");
            var updatedProduct = await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            await _unitOfWork.CommitAsync();
            
            return new NoContentResult("Ürün yumuşak silme ile kaldırıldı");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ürün silinirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> RestoreProductAsync(Guid id)
    {
        try
        {
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return new NotFoundResult("Ürün bulunamadı");
            }

            if (!product.IsDeleted)
            {
                return new ErrorResult("Ürün zaten aktif durumda");
            }

            product.Restore("system");
            var updatedProduct = await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            await _unitOfWork.CommitAsync();

            var productDto = _mapper.Map<ProductDto>(updatedProduct);
            return new SuccessResult<ProductDto>(productDto, "Ürün geri yüklendi");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Ürün geri yüklenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> UpdateStockAsync(Guid id, int newStock)
    {
        try
        {
            var product = await _unitOfWork.GetRepository<Product>().GetByIdAsync(id);
            if (product == null)
            {
                return new NotFoundResult<ProductDto>("Ürün bulunamadı");
            }

            product.UpdateStock(newStock);
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedBy = "system";

            var updatedProduct = await _unitOfWork.GetRepository<Product>().UpdateAsync(product);
            await _unitOfWork.CommitAsync();

            var productDto = _mapper.Map<ProductDto>(updatedProduct);
            return new SuccessResult<ProductDto>(productDto, "Stok güncellendi");
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Stok güncellenirken hata oluştu: {ex.Message}");
        }
    }

    public async Task<IResult> GetLowStockProductsAsync(int threshold = 10)
    {
        try
        {
            var allProducts = await _unitOfWork.GetRepository<Product>().GetAllAsync();
            var lowStockProducts = allProducts.Where(p => p.Stock <= threshold && p.Status == BaseApi.Domain.Enums.ProductStatus.Active).ToList();

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(lowStockProducts);
            return new SuccessResult<IEnumerable<ProductDto>>(productDtos);
        }
        catch (Exception ex)
        {
            return new ErrorResult($"Düşük stoklu ürünler getirilirken hata oluştu: {ex.Message}");
        }
    }
} 