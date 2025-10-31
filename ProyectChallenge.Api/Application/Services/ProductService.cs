using FluentValidation;
using ProyectChallenge.Api.Application.Dtos;
using ProyectChallenge.Api.Application.Middleware;
using ProyectChallenge.Api.Application.Services.Interface;
using ProyectChallenge.Api.Domain;
using ProyectChallenge.Api.Infrastructure.Interface;
using System.ComponentModel.DataAnnotations;

namespace ProyectChallenge.Api.Application.Services
{
    public class ProductService(IProductRepository repository, IValidator<CompareProductsRequest> validatorCompareProducst, IValidator<ProductRequest> validatorProduct) : IProductService
    {
        private readonly IProductRepository _repository = repository;
        private readonly IValidator<CompareProductsRequest> _validatorCompareProducst = validatorCompareProducst;
        private readonly IValidator<ProductRequest> _validatorProductFilter = validatorProduct;
        public async Task<List<Product>> GetAllAsync(CancellationToken ct)
        {
            var products = await _repository.GetAllAsync(ct);
            return products;
        }

        public async Task<List<Product>> GetByIdsAsync(List<string> ids, CancellationToken ct)
        {
            var products = await _repository.GetByIdsAsync(ids, ct);
            return products;
        }

        public async Task<List<Product>> GetWithFiltersAsync(ProductRequest request, CancellationToken ct)
        {
            var result = await _validatorProductFilter.ValidateAsync(request, ct);

            if (!result.IsValid)
                throw new FluentValidation.ValidationException(result.Errors);

            var products = await _repository.GetAllAsync(ct);

            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var lowerName = request.Name.ToLower();
                products = products
                    .Where(p => p.Name.ToLower().Contains(lowerName) || p.Description.ToLower().Contains(lowerName))
                    .ToList();
            }

            if (request.MinPrice.HasValue)
                products = products.Where(p => p.Price >= request.MinPrice.Value).ToList();

            if (request.MaxPrice.HasValue)
                products = products.Where(p => p.Price <= request.MaxPrice.Value).ToList();

            return products;
        }

        public async Task<CompareProductsResponse> CompareAsync(CompareProductsRequest request, CancellationToken ct)
        {
            var result = await _validatorCompareProducst.ValidateAsync(request, ct);

            if (!result.IsValid)
                throw new FluentValidation.ValidationException(result.Errors);

            var products = await _repository.GetByIdsAsync(request.Ids, ct);

            // Si no se encuentra ninguno
            if (products.Count == 0)
                throw new NotFoundAppException("No se encontraron productos con los IDs especificados.");

            var allKeys = products
                .SelectMany(p => p.TechnicalSpecifications.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(k => k)
                .ToList();

            var specMatrix = new Dictionary<string, List<string?>>();

            foreach (var key in allKeys)
            {
                specMatrix[key] = products
                    .Select(p => p.TechnicalSpecifications.TryGetValue(key, out var value) ? value : null)
                    .ToList();
            }

            return new CompareProductsResponse
            {
                Products = products,
                TechnicalSpecificationsKeys = allKeys,
                TechnicalSpecificationsCompare = specMatrix
            };
        }


    }
}
