using AutoMapper;
using FinanceApp.Server.Data;
using FinanceApp.Server.Models.DailyOpenClose;
using FinanceApp.Server.Models.Logo;
using FinanceApp.Server.Models.News;
using FinanceApp.Server.Models.StockChartData;
using FinanceApp.Server.Models.TickerDetails;
using FinanceApp.Server.Models.Tickers;
using FinanceApp.Server.Services.Interfaces;
using FinanceApp.Shared.Models;
using FinanceApp.Shared.Models.News;
using FinanceApp.Shared.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerList;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Services.Implementations;

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

    public async Task<int> SaveListItemsToDbAsync(IEnumerable<TickerListItemDto> tickerListItemDtos)
    {
        var tickerList = tickerListItemDtos
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
            return 0;
        // no need to update because historical data doesn't (at least shouldn't?) change

        var dailyOpenClose = _mapper.Map<DailyOpenClose>(dailyOpenCloseDto);
        dailyOpenClose.Ticker = ticker;

        await _context.DailyOpenCloses.AddAsync(dailyOpenClose);
        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<StockChartDataDto>> GetStockChartDataAsync(string ticker, string timespan,
        int multiplier, DateTime queryDate, DateTime from, DateTime to)
    {
        var chartDataFromDb = await _context.StockChartData
            .Where(cd => cd.Ticker == ticker && cd.Timespan == timespan
                                             && cd.Multiplier == multiplier && cd.QueryDate == queryDate
                                             && cd.Date >= from && cd.Date <= to)
            .ToListAsync();
        return _mapper.Map<IEnumerable<StockChartDataDto>>(chartDataFromDb);
    }

    public async Task<int> SaveStockChartDataAsync(IEnumerable<StockChartDataDto> stockChartDataDtoList, string ticker,
        string timespan, int multiplier, DateTime queryDate)
    {
        var stockChartDataList = stockChartDataDtoList.Select(d => new StockChartData
        {
            Ticker = ticker,
            Timespan = timespan,
            Multiplier = multiplier,
            QueryDate = queryDate,
            Date = d.Date,
            Open = d.Open,
            Low = d.Low,
            Close = d.Close,
            High = d.High,
            Volume = d.Volume
        }).ToList();

        var chartDataFromDb = await _context.StockChartData.AsNoTracking().ToListAsync();

        var notInDb = stockChartDataList
            .ExceptBy(
                chartDataFromDb.Select(cd => new { cd.Ticker, cd.Timespan, cd.Multiplier, cd.Date, cd.QueryDate }),
                cd => new { cd.Ticker, cd.Timespan, cd.Multiplier, cd.Date, cd.QueryDate })
            .ToList();

        if (notInDb.Any()) await _context.StockChartData.AddRangeAsync(notInDb);

        var toUpdate = stockChartDataList.Except(notInDb).ToList();

        if (toUpdate.Any()) _context.UpdateRange(toUpdate);

        return await _context.SaveChangesAsync();
    }

    public async Task<int> SaveNewsImagesAsync(IEnumerable<NewsResultImageDto> resultImageDtos)
    {
        var newsIdsFromDb = await _context.NewsResults.AsNoTracking().Select(nr => nr.IdNews).ToListAsync();
        foreach (var resultImageDto in resultImageDtos)
        {
            // skip if already in db
            if (newsIdsFromDb.Contains(resultImageDto.NewsResultDto.IdNews)) continue;

            var newsResult = _mapper.Map<NewsResult>(resultImageDto.NewsResultDto);

            // override publisher mapping if publisher is already in db

            var publisherFromDb = await _context.Publishers.FindAsync(resultImageDto.NewsResultDto.Publisher.Name);

            if (publisherFromDb != null) newsResult.Publisher = publisherFromDb;

            // map many-to-many NewsResult-TickerResults
            var tickersFromDb = await _context.Results
                .Where(r => resultImageDto.NewsResultDto.Tickers.Contains(r.Ticker))
                .ToListAsync();

            newsResult.TickerResults = tickersFromDb;

            foreach (var ticker in tickersFromDb) ticker.NewsResults.Add(newsResult);

            // add to db
            await _context.NewsResults.AddAsync(newsResult);
        }

        return await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<NewsResultImageDto>> GetRecentNewsAsync(string ticker, int count)
    {
        throw new NotImplementedException();
    }
}