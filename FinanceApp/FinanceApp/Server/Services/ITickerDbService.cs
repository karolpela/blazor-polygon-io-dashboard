using FinanceApp.Server.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerDetails;

namespace FinanceApp.Server.Services
{
    public interface ITickerDbService
    {
        public Task<int> SubscribeToTickerAsync(string ticker, string username);
        public Task<int> UnsubscribeFromTickerAsync(string ticker, string username);
        public Task<bool> IsOnWatchlistAsync(string ticker, string username);
        public Task<int> SaveToDbAsync(TickerResultsDto tickerResultsDto);
    }
}