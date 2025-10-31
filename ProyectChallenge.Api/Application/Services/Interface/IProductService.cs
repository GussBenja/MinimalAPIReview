using ProyectChallenge.Api.Application.Dtos;
using ProyectChallenge.Api.Domain;

namespace ProyectChallenge.Api.Application.Services.Interface
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync(CancellationToken ct);
        Task<List<Product>> GetByIdsAsync(List<string> ids, CancellationToken ct);
        Task<List<Product>> GetWithFiltersAsync(ProductRequest request, CancellationToken ct);
        Task<CompareProductsResponse> CompareAsync(CompareProductsRequest request, CancellationToken ct);

    }
}
