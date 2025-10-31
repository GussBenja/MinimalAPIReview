using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using ProyectChallenge.Api.Application.Dtos;
using ProyectChallenge.Api.Application.Services;
using ProyectChallenge.Api.Application.Services.Interface;
using ProyectChallenge.Api.Domain;
using ProyectChallenge.Api.Handlers;
using System.Xml.Linq;

namespace ProyectChallenge.Api.Endpoints
{
    public static class ProductsEndpoints
    {
        public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/products")
                           .WithTags("Products")
                           .RequireRateLimiting("fixed");

            // GET /api/products
            group.MapGet("all",
                 async ([FromServices] IProductService productService,
                         CancellationToken ct) =>
                 {
                     var products = await productService.GetAllAsync(ct);
                     return Results.Ok(products);
                 });

            group.MapGet("filter",
                    (
                     [FromServices] IProductService productService,
                     [FromQuery] string? name,
                     [FromQuery] decimal? minPrice,
                     [FromQuery] decimal? maxPrice,
                     [FromServices] ILoggerFactory loggerFactory,
                     CancellationToken ct)
                        => ProductsHandlers.GetWithFiltersAsync(productService, name, minPrice, maxPrice, loggerFactory, ct)
                    )
                    .WithSummary("Obtener productos filtrados")
                    .WithDescription("""
                    Devuelve productos aplicando filtros opcionales.

                    Parámetros:
                    - name: filtra por coincidencia parcial en el nombre o descripción.
                    - minPrice: precio mínimo.
                    - maxPrice: precio máximo.

                    Ejemplo:
                    GET /api/products?name=monitor&minPrice=100&maxPrice=500
                    """)
                    .Produces<List<Product>>(StatusCodes.Status200OK)
                    .Produces(StatusCodes.Status422UnprocessableEntity)
                    .Produces(StatusCodes.Status500InternalServerError);

            group.MapPost("ids",
                     async (
                         [FromBody] List<string> ids,
                         [FromServices] IProductService productService,
                         CancellationToken ct
                     ) =>
                     {
                         var products = await productService.GetByIdsAsync(ids, ct);
                         return Results.Ok(products);
                     })
                     .WithSummary("Obtener varios productos por ID")
                     .WithDescription("Devuelve los detalles de múltiples productos enviados en una lista de IDs. " +
                                      "Ejemplo de body:\n\n```json\n[\"prd-001\", \"prd-002\"]\n```")
                     .Accepts<List<string>>("application/json")
                     .WithOpenApi(operation =>
                     {
                         operation.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
                         {
                             Description = "Lista de IDs de productos",
                             Required = true,
                             Content =
                             {
                                ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                                {
                                    Example = new Microsoft.OpenApi.Any.OpenApiArray
                                    {
                                        new Microsoft.OpenApi.Any.OpenApiString("prd-001"),
                                        new Microsoft.OpenApi.Any.OpenApiString("prd-002")
                                    }
                                }
                             }
                         };
                         return operation;
                     });



            group.MapPost("compare",
                async (
                    [FromBody] CompareProductsRequest request,
                    [FromServices] IProductService productService,
                    [FromServices] IComparisonHistoryService historyService,
                    [FromServices] ILoggerFactory loggerFactory,
                    CancellationToken ct
                ) =>
                {
                    return await ProductsHandlers.CompareAsync(
                        request,
                        productService,
                        historyService,
                        loggerFactory,
                        ct
                    );
                }
                )
                .WithSummary("Comparar productos por ID")
                .WithDescription("""
                Recibe una lista de IDs y devuelve la información alineada para comparación.

                Body de ejemplo:
                {
                  "ids": ["prd-001", "prd-002"]
                }

                Respuesta de ejemplo:
                {
                  "products": [
                    { "id": "prd-001", "name": "Monitor 27\" 144Hz", ... },
                    { "id": "prd-002", "name": "Monitor 24\" 75Hz", ... }
                  ],
                  "technicalSpecificationsKeys": ["panel", "resolution", "refreshRate", "ports"],
                  "technicalSpecificationsCompare": {
                    "panel": ["IPS", "VA"],
                    "resolution": ["2560x1440", "1920x1080"],
                    "refreshRate": ["144Hz", "75Hz"],
                    "ports": ["HDMI, DisplayPort", "HDMI"]
                  }
                }
                """)
                .Accepts<CompareProductsRequest>("application/json")
                .Produces<CompareProductsResponse>(StatusCodes.Status200OK)
                .WithOpenApi(operation =>
                {
                    operation.RequestBody = new Microsoft.OpenApi.Models.OpenApiRequestBody
                    {
                        Description = "IDs de productos a comparar",
                        Required = true,
                        Content =
                        {
                            ["application/json"] = new Microsoft.OpenApi.Models.OpenApiMediaType
                            {
                                Example = new Microsoft.OpenApi.Any.OpenApiObject
                                {
                                    ["ids"] = new Microsoft.OpenApi.Any.OpenApiArray
                                    {
                                        new Microsoft.OpenApi.Any.OpenApiString("prd-001"),
                                        new Microsoft.OpenApi.Any.OpenApiString("prd-002")
                                    }
                                }
                            }
                        }
                    };
                    return operation;
                });



            group.MapGet("compare/history",
                    (
                        [FromServices] IComparisonHistoryService historyService,
                        [FromQuery] int? take
                    ) =>
                    {
                        var items = historyService.GetLast(take ?? 10);
                        return Results.Ok(items);
                    }
                )
                .WithSummary("Obtener historial de comparaciones")
                .WithDescription("""
                Devuelve las últimas comparaciones realizadas.

                Parámetros:
                - take (opcional): cuántos registros querés. Default: 10.

                Ejemplo:
                GET /api/products/compare/history?take=5
                """)
                .Produces<List<ComparisonHistoryEntry>>(StatusCodes.Status200OK);

            return app;
        }
    }
}
