using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProyectChallenge.Api.Application.Dtos;
using ProyectChallenge.Api.Application.Services.Interface;
using ProyectChallenge.Api.Domain;
using System.ComponentModel.DataAnnotations;

namespace ProyectChallenge.Api.Handlers
{
    public static class ProductsHandlers
    {
        public static async Task<IResult> GetAllAsync(IProductService productService,CancellationToken ct)
        {
            List<Product> products = await productService.GetAllAsync(ct);
            return Results.Ok(products);
        }

        public static async Task<IResult> GetByIdAsync(List<string> ids, IProductService productService,CancellationToken ct)
        {
            var list = await productService.GetByIdsAsync(ids, ct);
            var product = list.FirstOrDefault();

            return product is not null?   Results.Ok(product) : throw new ArgumentException("");
        }

        public static async Task<IResult> GetWithFiltersAsync(
            IProductService productService,
            string? name,decimal? minPrice,decimal? maxPrice, ILoggerFactory loggerFactory, CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("Audit");

            var request = new ProductRequest
            {
                Name = name ?? string.Empty,
                MinPrice = minPrice ?? 0,
                MaxPrice = maxPrice ?? 0
            };
           
            var products = await productService.GetWithFiltersAsync(request, ct);

            logger.LogInformation("[AUDIT] GET Products with filters -> Name: {Name}, MinPrice: {Min}, MaxPrice: {Max}, Found: {Count}",
        name ?? "(none)", minPrice, maxPrice, products.Count);

            return Results.Ok(products);
        }

        public static async Task<IResult> CompareAsync(
            CompareProductsRequest request,
            IProductService productService,
            IComparisonHistoryService historyService,
            ILoggerFactory loggerFactory,
            CancellationToken ct)
        {
            var logger = loggerFactory.CreateLogger("Audit");

            var compareResult = await productService.CompareAsync(request, ct);

            logger.LogInformation("[AUDIT] COMPARE -> IDs: {Ids}, Products Compared: {Count}",string.Join(", ", request.Ids), compareResult.Products.Count);

            var entry = new ComparisonHistoryEntry
            {
                TimestampUtc = DateTime.UtcNow,
                ProductIds = request.Ids,
                ProductNames = compareResult.Products
                    .Select(p => p.Name)
                    .ToList()
            };

            historyService.AddEntry(entry);

            return Results.Ok(compareResult);
        }
    }
}
