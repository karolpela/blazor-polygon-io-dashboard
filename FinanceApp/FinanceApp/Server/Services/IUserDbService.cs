namespace FinanceApp.Server.Services
{
    public interface IUserDbService
    {
        public Task<int> AddToWatchlistAsync(string userId, string ticker);
    }
}
