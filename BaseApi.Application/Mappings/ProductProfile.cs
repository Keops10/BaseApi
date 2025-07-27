using AutoMapper;
using BaseApi.Application.Dtos;
using BaseApi.Domain.Entities;

namespace BaseApi.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        // Entity to DTO
        CreateMap<Product, ProductDto>().ReverseMap();

        // DTO to Entity
        CreateMap<CreateProductDto, Product>().ReverseMap();

        CreateMap<UpdateProductDto, Product>().ReverseMap();
    }
} 