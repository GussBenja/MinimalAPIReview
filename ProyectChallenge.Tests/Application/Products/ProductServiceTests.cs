using FluentAssertions;
using FluentValidation;
using Moq;
using ProyectChallenge.Api.Application.Dtos;
using ProyectChallenge.Api.Application.Services;
using ProyectChallenge.Api.Application.Services.Interface;
using ProyectChallenge.Api.Application.Validation;
using ProyectChallenge.Api.Domain;
using ProyectChallenge.Api.Infrastructure.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProyectChallenge.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _repoMock;
        private readonly IProductService _service;
         readonly IValidator<CompareProductsRequest> _validatorCompareProduct;
         readonly IValidator<ProductRequest> _validatorProductFilter;
        public ProductServiceTests()
        {
            var fakeProducts = new List<Product>
            {
                new ()
                {
                    Id = "prd-001",
                    Name = "Monitor 27\" 144Hz",
                    Description = "Monitor gamer IPS 144Hz",
                    Price = 250m,
                    Rating = 4.5,
                    TechnicalSpecifications = new Dictionary<string, string>
                    {
                        ["panel"] = "IPS",
                        ["hz"] = "144"
                    }
                },
                new ()
                {
                    Id = "prd-002",
                    Name = "Monitor 24\" 75Hz",
                    Description = "Monitor oficina VA 75Hz",
                    Price = 130m,
                    Rating = 4.2,
                    TechnicalSpecifications = new Dictionary<string, string>
                    {
                        ["panel"] = "VA",
                        ["hz"] = "75"
                    }
                },
                new() {
                    Id = "prd-003",
                    Name = "Mouse Logitech Pro",
                    Description = "Mouse gamer inalámbrico",
                    Price = 99m,
                    Rating = 4.8,
                    TechnicalSpecifications = new Dictionary<string, string>
                    {
                        ["dpi"] = "25000"
                    }
                }
            };
            _validatorProductFilter = new ProductRequestValidator();
            _validatorCompareProduct = new CompareProductsRequestValidator();

            _repoMock = new Mock<IProductRepository>();
            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(fakeProducts);

            _repoMock.Setup(r => r.GetByIdsAsync(It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((List<string> ids, CancellationToken _) =>
                {
                    var set = new HashSet<string>(ids);
                    return fakeProducts.FindAll(p => set.Contains(p.Id));
                });

            _service = new ProductService(_repoMock.Object, _validatorCompareProduct, _validatorProductFilter);
        }

        [Fact]
        public async Task GetWithFiltersAsync_FiltersByName_AndPriceRange()
        {
            // arrange
            var request = new ProductRequest
            {
                Name = "Monitor",    
                MinPrice = 150m, 
                MaxPrice = 300m
            };

            // act
            var result = await _service.GetWithFiltersAsync(request, CancellationToken.None);

            // assert
            result.Should().HaveCount(1);
            result[0].Id.Should().Be("prd-001"); 
        }

        [Fact]
        public async Task GetWithFiltersAsync_ThrowsException_WhenMinPriceGreaterThanMaxPrice()
        {
            // arrange
            var request = new ProductRequest
            {
                Name = "Monitor",
                MinPrice = 300m,
                MaxPrice = 100m
            };

            // act
            var act = async () => await _service.GetWithFiltersAsync(request, CancellationToken.None);

            // assert
            await act.Should()
                .ThrowAsync<Exception>()
                 .WithMessage("*precio máximo no puede ser menor*");
        }

    }
}
