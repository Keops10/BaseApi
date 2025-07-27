using BaseApi.Application.Dtos;
using MediatR;

namespace BaseApi.Application.Features.Product.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ProductDto?>
{
    public Guid Id { get; set; }
} 