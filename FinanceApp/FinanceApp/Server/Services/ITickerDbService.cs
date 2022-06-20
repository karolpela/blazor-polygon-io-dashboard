using FinanceApp.Shared.Models;
using FinanceApp.Shared.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerList;

namespace FinanceApp.Server.Services;

public interface ITickerDbService
{
    public Task<IEnumerable<TickerListItemDto>> GetTickerListItemsAsync();
    public Task<int> SaveListItemsToDbAsync(IEnumerable<TickerListItemDto> tickerListItemDto);

    public Task<TickerResultsDto?> GetTickerResultsAsync(string ticker);
    public Task<int> SaveResultsToDbAsync(TickerResultsDto tickerResultsDto);

    public Task<LogoDto?> GetLogoAsync(string ticker);
    public Task<int> UpdateLogoAsync(LogoDto logo);

    public Task<int> SubscribeToTickerAsync(string ticker, string username);
    public Task<int> UnsubscribeFromTickerAsync(string ticker, string username);

    public Task<bool> IsOnWatchlistAsync(string ticker, string username);

    public Task<DailyOpenCloseDto?> GetDailyOpenCloseAsync(string ticker, string from);
    public Task<int> SaveDailyOpenCloseToDbAsync(string ticker, DailyOpenCloseDto dailyOpenCloseDto);
}