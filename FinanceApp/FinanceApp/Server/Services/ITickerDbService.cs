using FinanceApp.Server.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerDetails;

namespace FinanceApp.Server.Services
{
    public interface ITickerDbService
    {
        public Task<int> SaveToDbAsync(TickerResultsDto tickerResultsDto);
    }
}