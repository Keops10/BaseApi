using BaseApi.Abstractions.Services;
using BaseApi.Abstractions.Results;
using BaseApi.Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseApi.API.Controllers.Base;

namespace BaseApi.API.Controllers;

/// <summary>
/// Ürün yönetimi için API endpoint'leri
/// </summary>
[Route("api/[controller]")]
[Authorize]
public class ProductsController : BaseController
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Tüm aktif ürünleri listeler
    /// </summary>
    /// <returns>Ürün listesi</returns>
    /// <response code="200">Ürünler başarıyla getirildi</response>
    /// <response code="401">Yetkilendirme gerekli</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllProducts()
    {
        var result = await _productService.GetAllProductsAsync();
        return CreateActionResult(result);
    }

    /// <summary>
    /// Belirtilen ID'ye sahip ürünü getirir
    /// </summary>
    /// <param name="id">Ürün ID'si</param>
    /// <returns>Ürün detayları</returns>
    /// <response code="200">Ürün başarıyla getirildi</response>
    /// <response code="404">Ürün bulunamadı</response>
    /// <response code="401">Yetkilendirme gerekli</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var result = await _productService.GetProductByIdAsync(id);
        return CreateActionResult(result);
    }

    /// <summary>
    /// Yeni bir ürün oluşturur
    /// </summary>
    /// <param name="createProductDto">Ürün oluşturma verileri</param>
    /// <returns>Oluşturulan ürün</returns>
    /// <response code="201">Ürün başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri</response>
    /// <response code="401">Yetkilendirme gerekli</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var result = await _productService.CreateProductAsync(createProductDto);
        return CreateActionResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        var result = await _productService.UpdateProductAsync(id, updateProductDto);
        return CreateActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await _productService.DeleteProductAsync(id);
        return CreateActionResult(result);
    }

    [HttpPut("{id}/restore")]
    public async Task<IActionResult> RestoreProduct(Guid id)
    {
        var result = await _productService.RestoreProductAsync(id);
        return CreateActionResult(result);
    }

    [HttpPut("{id}/stock")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] int newStock)
    {
        var result = await _productService.UpdateStockAsync(id, newStock);
        return CreateActionResult(result);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockProducts([FromQuery] int threshold = 10)
    {
        var result = await _productService.GetLowStockProductsAsync(threshold);
        return CreateActionResult(result);
    }
} 