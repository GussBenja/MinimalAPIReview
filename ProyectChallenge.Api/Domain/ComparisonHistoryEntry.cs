namespace ProyectChallenge.Api.Domain
{
    public class ComparisonHistoryEntry
    {
        public DateTime TimestampUtc { get; set; }
        public List<string> ProductIds { get; set; } = new();
        public List<string> ProductNames { get; set; } = new();
    }
}