using ProyectChallenge.Api.Domain;
using ProyectChallenge.Api.Infrastructure.Interface;
using System.Text.Json;

namespace ProyectChallenge.Api.Infrastructure
{
    public class ProductRepository(IWebHostEnvironment env) : IProductRepository
    {
        private readonly string _path = Path.Combine(env.ContentRootPath, "data", "products.json");
        private List<Product>? _cache;

        private async Task<List<Product>> EnsureCacheAsync(CancellationToken cancellationToken)
        {
            if (_cache is not null)
                return _cache;

            if (!File.Exists(_path))
            {
                _cache = [];
                return _cache;
            }

            await using var fs = File.OpenRead(_path);
            _cache = await JsonSerializer.DeserializeAsync<List<Product>>(fs,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                cancellationToken
            ) ?? [];

            return _cache;
        }

        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await EnsureCacheAsync(cancellationToken);
        }

        public async Task<List<Product>> GetByIdsAsync(List<string> ids, CancellationToken cancellationToken)
        {
            var set = new HashSet<string>(ids, StringComparer.OrdinalIgnoreCase);
            var all = await EnsureCacheAsync(cancellationToken);
            return all.Where(p => set.Contains(p.Id)).ToList();
        }
    }
}