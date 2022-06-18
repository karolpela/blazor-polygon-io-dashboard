using AutoMapper;
using FinanceApp.Server.Data;
using FinanceApp.Server.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerDetails;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Services
{
    public class TickerDbService : ITickerDbService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TickerDbService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> SaveToDbAsync(TickerResultsDto tickerResultsDto)
        {
            var tickerResults = _mapper.Map<TickerResults>(tickerResultsDto);
            
            var tickerResultsFromDb =
                await _context.Results.AsNoTracking()
                    .Where(e => e.Ticker == tickerResults.Ticker)
                    .SingleOrDefaultAsync();
            
            if (tickerResultsFromDb == null)
            {
                await _context.Results.AddAsync(tickerResults);
            }
            else
            {
                _context.Entry(tickerResultsFromDb).CurrentValues.SetValues(tickerResults);
                _context.Entry(tickerResultsFromDb).State = EntityState.Modified;
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> SubscribeToTickerAsync(string ticker, string username)
        {
            var userFromDb = await _context.Users.Where(u => u.UserName == username).SingleOrDefaultAsync();
            
            if (userFromDb == null)
            {
                return 0;
            }
            
            var tickerFromDb = await _context.Results.FindAsync(ticker);
            
            if (tickerFromDb == null)
            {
                return 0;
            }
            
            userFromDb.TickerWatchlist.Add(tickerFromDb);
            
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UnsubscribeFromTickerAsync(string ticker, string username)
        {
            var userFromDb = await _context.Users.Where(u => u.UserName == username)
                .Include(u => u.TickerWatchlist).SingleOrDefaultAsync();

            
            if (userFromDb == null)
            {
                return 0;
            }
            
            var tickerFromDb = await _context.Results.Where(t => t.Ticker == ticker)
                .Include(t => t.ApplicationUsers).SingleOrDefaultAsync();
            
            if (tickerFromDb == null)
            {
                return 0;
            }
            
            // TODO handle a case where ticker is not on user's watchlist
            userFromDb.TickerWatchlist.Remove(tickerFromDb);
            tickerFromDb.ApplicationUsers.Remove(userFromDb);
            
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> IsOnWatchlistAsync(string ticker, string username)
        {
            return await _context.Results
                .Include(r => r.ApplicationUsers)
                .Where(r => r.Ticker == ticker &&
                            r.ApplicationUsers.Select(au => au.UserName).Contains(username))
                .AnyAsync();
        }
    }
}