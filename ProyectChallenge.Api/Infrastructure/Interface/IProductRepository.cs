using ProyectChallenge.Api.Domain;

namespace ProyectChallenge.Api.Infrastructure.Interface
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken);
        Task<List<Product>> GetByIdsAsync(List<string> ids, CancellationToken cancellationToken);
    }
}
