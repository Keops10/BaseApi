using BaseApi.Abstractions.Repositories;
using BaseApi.Abstractions.Services;
using BaseApi.Application.Services;
using BaseApi.Persistence.Repositories;
using FluentValidation;
using System.Reflection;

namespace BaseApi.API.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(BaseApi.Application.Mappings.UserProfile).Assembly));

        // MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BaseApi.Application.Mappings.UserProfile).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(BaseApi.Application.Mappings.UserProfile).Assembly);

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
} 