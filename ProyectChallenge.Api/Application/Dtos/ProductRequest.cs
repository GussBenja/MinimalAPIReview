namespace ProyectChallenge.Api.Application.Dtos
{
    public class ProductRequest
    {
        public string? Name { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
