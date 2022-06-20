using AutoMapper;
using FinanceApp.Server.Data;
using FinanceApp.Server.Models.DailyOpenClose;
using FinanceApp.Server.Models.Logo;
using FinanceApp.Server.Models.TickerDetails;
using FinanceApp.Server.Models.Tickers;
using FinanceApp.Shared.Models;
using FinanceApp.Shared.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerList;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Services;

public class TickerDbService : ITickerDbService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public TickerDbService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<int> SaveResultsToDbAsync(TickerResultsDto tickerResultsDto)
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

    public async Task<int> SaveListItemsToDbAsync(IEnumerable<TickerListItemDto> itemListDto)
    {
        var tickerList = itemListDto
            .Select(i => _mapper.Map<TickerListItem>(i))
            .ToList();

        var tickerListFromDb = await _context.TickerListItems.AsNoTracking().ToListAsync();

        var notInDb = tickerList
            .ExceptBy(tickerListFromDb.Select(i => i.Ticker), i => i.Ticker)
            .ToList();

        if (notInDb.Any()) await _context.TickerListItems.AddRangeAsync(notInDb);

        var toUpdate = tickerList.Except(notInDb).ToList();

        if (toUpdate.Any()) _context.UpdateRange(toUpdate);

        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<TickerListItemDto>> GetTickerListItemsAsync()
    {
        return await _context.TickerListItems
            .Select(i => _mapper.Map<TickerListItemDto>(i))
            .ToListAsync();
    }

    public async Task<TickerResultsDto?> GetTickerResultsAsync(string ticker)
    {
        var resultsFromDb = await _context.Results.FindAsync(ticker);
        return _mapper.Map<TickerResultsDto>(resultsFromDb);
    }

    public async Task<LogoDto?> GetLogoAsync(string ticker)
    {
        var logoFromDb = await _context.Logos.FindAsync(ticker);
        return _mapper.Map<LogoDto>(logoFromDb);
    }

    public async Task<int> UpdateLogoAsync(LogoDto logoDto)
    {
        if (await _context.Logos.AsNoTracking().Where(l => l.Ticker == logoDto.Ticker).AnyAsync()) return 0;

        var tickerFromDb = await _context.Results.FindAsync(logoDto.Ticker);
        if (tickerFromDb != null)
        {
            var logo = _mapper.Map<Logo>(logoDto);
            tickerFromDb.Logo = logo;
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<int> SubscribeToTickerAsync(string ticker, string username)
    {
        var userFromDb = await _context.Users.Where(u => u.UserName == username).SingleOrDefaultAsync();

        if (userFromDb == null) return 0;

        var tickerFromDb = await _context.Results.FindAsync(ticker);

        if (tickerFromDb == null) return 0;

        userFromDb.TickerWatchlist.Add(tickerFromDb);

        return await _context.SaveChangesAsync();
    }

    public async Task<int> UnsubscribeFromTickerAsync(string ticker, string username)
    {
        var userFromDb = await _context.Users.Where(u => u.UserName == username)
            .Include(u => u.TickerWatchlist).SingleOrDefaultAsync();


        if (userFromDb == null) return 0;

        var tickerFromDb = await _context.Results.Where(t => t.Ticker == ticker)
            .Include(t => t.ApplicationUsers).SingleOrDefaultAsync();

        if (tickerFromDb == null) return 0;

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

    public async Task<DailyOpenCloseDto?> GetDailyOpenCloseAsync(string ticker, string from)
    {
        var openCloseFromDb = await _context.DailyOpenCloses
            .Where(doc => doc.Ticker == ticker && doc.From == from).SingleOrDefaultAsync();
        return _mapper.Map<DailyOpenCloseDto>(openCloseFromDb);
    }

    public async Task<int> SaveDailyOpenCloseToDbAsync(string ticker, DailyOpenCloseDto dailyOpenCloseDto)
    {
        if (await _context.DailyOpenCloses
                .AsNoTracking()
                .Where(doc => doc.Ticker == ticker && doc.From == dailyOpenCloseDto.From).AnyAsync())
            // no need to update because historical data doesn't (at least shouldn't?) change
            return 0;


        var dailyOpenClose = _mapper.Map<DailyOpenClose>(dailyOpenCloseDto);
        dailyOpenClose.Ticker = ticker;

        await _context.DailyOpenCloses.AddAsync(dailyOpenClose);
        return await _context.SaveChangesAsync();
    }
}