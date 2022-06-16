using FinanceApp.Server.Models.TickerDetails;

namespace FinanceApp.Server.Services
{
    public interface ITickerDbService
    {
        public Task<int> SaveToDbAsync(TickerDetails tickerDetails);
    }
}
