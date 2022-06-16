using FinanceApp.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Services
{
    public class UserDbService : IUserDbService
    {
        private readonly ApplicationDbContext _context;

        public UserDbService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddToWatchlistAsync(string userId, string ticker)
        {
            var userFromDb = await _context.Users.FindAsync(userId);

            if (userFromDb == null)
            {
                return 0;
            }

            var tickerFromDb = await _context.TickerDetails.FindAsync(ticker);

            if (tickerFromDb == null)
            {
                return 0;
            }

            userFromDb.TickerWatchlist.Add(tickerFromDb);

            return await _context.SaveChangesAsync();
        }
    }
}
