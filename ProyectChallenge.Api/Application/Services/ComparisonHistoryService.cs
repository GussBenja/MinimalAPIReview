using ProyectChallenge.Api.Application.Services.Interface;
using ProyectChallenge.Api.Domain;

namespace ProyectChallenge.Api.Application.Services
{
    public class ComparisonHistoryService : IComparisonHistoryService
    {
        // lista en memoria
        private readonly List<ComparisonHistoryEntry> _history = new();

        public void AddEntry(ComparisonHistoryEntry entry)
        {
            _history.Add(entry);

            if (_history.Count > 100)
            {
                _history.RemoveRange(0, _history.Count - 100);
            }
        }

        public List<ComparisonHistoryEntry> GetLast(int take)
        {
            return _history
                .OrderByDescending(h => h.TimestampUtc)
                .Take(take)
                .ToList();
        }
    }
}
