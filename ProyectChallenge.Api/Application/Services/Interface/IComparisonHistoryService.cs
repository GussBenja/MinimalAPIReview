using ProyectChallenge.Api.Domain;

namespace ProyectChallenge.Api.Application.Services.Interface
{
    public interface IComparisonHistoryService
    {
        void AddEntry(ComparisonHistoryEntry entry);
        List<ComparisonHistoryEntry> GetLast(int take);
    }
}
