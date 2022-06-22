using Duende.IdentityServer.Extensions;
using FinanceApp.Server.Models.Bars;
using FinanceApp.Server.Models.StockChartData;
using FinanceApp.Server.Services;
using FinanceApp.Shared.Models;
using FinanceApp.Shared.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Server.Controllers;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TickersController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ITickerDbService _tickerDbService;

    public TickersController(IConfiguration configuration, IHttpClientFactory clientFactory,
        ITickerDbService tickerDbService)
    {
        _configuration = configuration;
        _clientFactory = clientFactory;
        _tickerDbService = tickerDbService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickersAsync()
    {
        var client = _clientFactory.CreateClient();
        List<TickerListItemDto> itemList = new();

        // Try to get new data from Polygon
        try
        {
            throw new HttpRequestException();
            var tickerListDto = await client.GetFromJsonAsync<TickerListDto>(
                "https://api.polygon.io/v3/reference/tickers?type=CS" +
                "&market=stocks&exchange=XNAS" +
                "&active=true&sort=ticker" +
                "&order=asc" +
                "&limit=1000" +
                $"&apiKey={_configuration["Polygon:ApiKey"]}");
            itemList = tickerListDto!.Results;
            // Save updated list to db
            await _tickerDbService.SaveListItemsToDbAsync(itemList);
        }
        catch (HttpRequestException)
        {
            // In case Polygon is not available, get Ticker list from db
            itemList.AddRange(await _tickerDbService.GetTickerListItemsAsync());
        }

        return Ok(itemList);
    }

    [HttpGet("{ticker}")]
    public async Task<IActionResult> GetTickerAsync(string ticker)
    {
        var client = _clientFactory.CreateClient();

        TickerResultsDto? tickerResultsDto;

        try
        {
            throw new HttpRequestException();
            var tickerDetailsDto = await client.GetFromJsonAsync<TickerDetailsDto>(
                $"https://api.polygon.io/v3/reference/tickers/{ticker}" +
                $"?apiKey={_configuration["Polygon:ApiKey"]}");
            tickerResultsDto = tickerDetailsDto!.TickerResults;
            await _tickerDbService.SaveResultsToDbAsync(tickerResultsDto);
        }
        catch (HttpRequestException)
        {
            tickerResultsDto = await _tickerDbService.GetTickerResultsAsync(ticker);
        }

        LogoDto? logoDto = null;

        if (tickerResultsDto == null)
        {
            logoDto = await _tickerDbService.GetLogoAsync(ticker);
        }
        else if (tickerResultsDto.Branding != null)
        {
            try
            {
                var logoArray = await client.GetByteArrayAsync(
                    $"{tickerResultsDto.Branding.LogoUrl}" +
                    $"?apiKey={_configuration["Polygon:ApiKey"]}");
                var logoFormat = Path.GetExtension(tickerResultsDto.Branding.LogoUrl)[1..];

                logoDto = new LogoDto(ticker, logoArray, logoFormat);
                await _tickerDbService.UpdateLogoAsync(logoDto);
            }
            catch (HttpRequestException)
            {
                logoDto = await _tickerDbService.GetLogoAsync(ticker);
            }
        }
        else
        {
            logoDto = await _tickerDbService.GetLogoAsync(ticker);
        }

        if (tickerResultsDto == null && logoDto == null) return NotFound();

        var tickerResultsLogoDto = new TickerResultsLogoDto(tickerResultsDto, logoDto);
        return Ok(tickerResultsLogoDto);
    }

    [HttpGet("{ticker}/open-close/{from}")]
    public async Task<IActionResult> GetTickerOpenCloseAsync(string ticker, string from)
    {
        var client = _clientFactory.CreateClient();

        DailyOpenCloseDto? dailyOpenCloseDto;
        try
        {
            throw new HttpRequestException();

            dailyOpenCloseDto = await client.GetFromJsonAsync<DailyOpenCloseDto>(
                $"https://api.polygon.io/v1/open-close/{ticker}/{from}" +
                $"?apiKey={_configuration["Polygon:ApiKey"]}");
            if (dailyOpenCloseDto != null)
                await _tickerDbService.SaveDailyOpenCloseToDbAsync(ticker, dailyOpenCloseDto);
        }
        catch (HttpRequestException)
        {
            dailyOpenCloseDto = await _tickerDbService.GetDailyOpenCloseAsync(ticker, from);
        }

        return dailyOpenCloseDto != null ? Ok(dailyOpenCloseDto) : NotFound();
    }

    [AllowAnonymous]
    [HttpPost("{ticker}/users")]
    public async Task<IActionResult> SubscribeToTicker(string ticker, [FromBody] string username)
    {
        var result = await _tickerDbService.SubscribeToTickerAsync(ticker, username);

        if (result == 0) return BadRequest();

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpDelete("{ticker}/users/{username}")]
    public async Task<IActionResult> UnsubscribeFromTicker(string ticker, string username)
    {
        var result = await _tickerDbService.UnsubscribeFromTickerAsync(ticker, username);

        if (result == 0) return BadRequest();

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{ticker}/on-watchlist/{username}")]
    public async Task<IActionResult> IsOnWatchlistAsync(string ticker, string username)
    {
        var isOnWatchlist = await _tickerDbService.IsOnWatchlistAsync(ticker, username);
        return isOnWatchlist ? Ok() : NotFound();
    }

    [HttpGet("{ticker}/bars")]
    public async Task<IActionResult> GetBarsAsync(string ticker, string timespan, int multiplier, long fromUnix, long toUnix)
    {
        var client = _clientFactory.CreateClient();


        var fromOffset = DateTimeOffset.FromUnixTimeMilliseconds(fromUnix);
        var toOffset = DateTimeOffset.FromUnixTimeMilliseconds(toUnix);

        // exchange opens at 1:30 PM UTC and closes at 9:30 PM UTC
        var fromOffsetAdjusted = new DateTimeOffset(fromOffset.Year, fromOffset.Month, fromOffset.Day, 13, 30, 0, TimeSpan.Zero);
        var toOffsetAdjusted = new DateTimeOffset(toOffset.Year, toOffset.Month, toOffset.Day, 19, 30, 0, TimeSpan.Zero);

        var fromOffsetAdjustedUnix = fromOffsetAdjusted.ToUnixTimeMilliseconds();
        var toOffsetAdjustedUnix = toOffsetAdjusted.ToUnixTimeMilliseconds();

        List<StockChartDataDto>? chartDataDtoList;
        Bars? bars;
        try
        {
            bars = await client.GetFromJsonAsync<Bars>(
                $"https://api.polygon.io/v2/aggs/ticker/{ticker}/range/{multiplier}/{timespan}/{fromOffsetAdjustedUnix}/{toOffsetAdjustedUnix}" +
                "?adjusted=true" +
                "&sort=asc" +
                "&limit=5000" +
                $"&apiKey={_configuration["Polygon:ApiKey"]}");
        }
        catch (HttpRequestException)
        {
            // get from db
            chartDataDtoList = (await _tickerDbService.GetStockChartDataAsync(ticker, timespan, multiplier,
                fromOffsetAdjusted.DateTime, toOffsetAdjusted.DateTime)).ToList();
            if (chartDataDtoList.IsNullOrEmpty()) return NotFound();
            return Ok(chartDataDtoList);
        }

        if (bars == null) return NotFound();

        chartDataDtoList = new List<StockChartDataDto>();
        var date = fromOffsetAdjusted;
        foreach (var result in bars.Results)
        {
            // skip weekends
            date = date.DayOfWeek switch
            {
                DayOfWeek.Saturday => date.AddDays(2),
                DayOfWeek.Sunday => date.AddDays(1),
                _ => date
            };

            chartDataDtoList.Add(new StockChartDataDto
            {
                Date = date.DateTime,
                Open = result.O,
                Low = result.L,
                Close = result.C,
                High = result.H,
                Volume = result.V
            }); 

            date = timespan switch
            {
                "minute" => date.AddMinutes(multiplier),
                "hour" => date.AddHours(multiplier),
                "day" => date.AddDays(multiplier),
                "week" => date.AddDays(7 * multiplier),
                _ => date,
            };
        }
        // save to db
        await _tickerDbService.SaveStockChartDataAsync(chartDataDtoList, ticker, timespan, multiplier);
        return Ok(chartDataDtoList);
    }
}