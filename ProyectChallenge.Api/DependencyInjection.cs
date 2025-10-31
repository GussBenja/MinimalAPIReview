using FluentValidation;
using ProyectChallenge.Api.Application.Dtos;
using ProyectChallenge.Api.Application.Services;
using ProyectChallenge.Api.Application.Services.Interface;
using ProyectChallenge.Api.Application.Validation;
using ProyectChallenge.Api.Infrastructure;
using ProyectChallenge.Api.Infrastructure.Interface;

namespace ProyectChallenge.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInyection(this IServiceCollection services)
    {
        services.AddSingleton<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>(); 
        services.AddSingleton<IComparisonHistoryService, ComparisonHistoryService>();

        services.AddScoped<IValidator<ProductRequest>, ProductRequestValidator>(); 
        services.AddScoped<IValidator<CompareProductsRequest>, CompareProductsRequestValidator>();

        return services;
    }
}
