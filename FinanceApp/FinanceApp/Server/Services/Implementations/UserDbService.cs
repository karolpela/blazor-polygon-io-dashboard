using AutoMapper;
using FinanceApp.Server.Data;
using FinanceApp.Server.Services.Interfaces;
using FinanceApp.Shared.Models.TickerList;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Services.Implementations
{
    public class UserDbService : IUserDbService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserDbService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TickerListItemDto>> GetUserWatchlistAsync(string username)
        {
            var watchlistFromDb = await _context.Users
                .Include(au => au.TickerWatchlist)
                .ToListAsync();
            return _mapper.Map<IEnumerable<TickerListItemDto>>(watchlistFromDb);
        }

        public async Task<bool> IsOnWatchListAsync(string username, string ticker)
        {
            return await _context.Users
                .Include(au => au.TickerWatchlist)
                .Where(au => au.UserName == username && au.TickerWatchlist.Select(t => t.Ticker).Contains(ticker))
                .AnyAsync();
        }
    }
}
