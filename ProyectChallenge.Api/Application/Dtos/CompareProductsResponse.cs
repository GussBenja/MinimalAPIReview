using ProyectChallenge.Api.Domain;

namespace ProyectChallenge.Api.Application.Dtos;

public class CompareProductsResponse
{
    public List<Product> Products { get; set; } = new();
    public List<string> TechnicalSpecificationsKeys { get; set; } = new();
    public Dictionary<string, List<string?>> TechnicalSpecificationsCompare { get; set; } = new();
}
