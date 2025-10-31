namespace ProyectChallenge.Api.Domain
{
    public class Product
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public double Rating { get; set; }

        public Dictionary<string, string> TechnicalSpecifications { get; set; } = new Dictionary<string, string>();
    }
}
