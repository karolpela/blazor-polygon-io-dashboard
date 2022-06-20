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
            var tickerListDto = await client.GetFromJsonAsync<TickerListDto>(
                "https://api.polygon.io/v3/reference/tickers?type=CS" +
                "&market=stocks&exchange=XNAS&active=true&sort=ticker&order=asc&limit=1000" +
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
            var tickerDetailsDto = await client.GetFromJsonAsync<TickerDetailsDto>(
                $"https://api.polygon.io/v3/reference/tickers/{ticker}?apiKey={_configuration["Polygon:ApiKey"]}");
            tickerResultsDto = tickerDetailsDto!.TickerResults;
            await _tickerDbService.SaveResultsToDbAsync(tickerResultsDto);
        }
        catch (HttpRequestException)
        {
            tickerResultsDto = await _tickerDbService.GetTickerResultsAsync(ticker);
        }

        if (tickerResultsDto == null) return NotFound();

        LogoDto? logoDto = null;

        if (tickerResultsDto.Branding != null)
            try
            {
                var logoArray = await client.GetByteArrayAsync(
                    $"{tickerResultsDto.Branding.LogoUrl}?apiKey={_configuration["Polygon:ApiKey"]}");
                var logoFormat = Path.GetExtension(tickerResultsDto.Branding.LogoUrl)[1..];

                logoDto = new LogoDto(ticker, logoArray, logoFormat);
                await _tickerDbService.UpdateLogoAsync(logoDto);
            }
            catch (HttpRequestException)
            {
                logoDto = await _tickerDbService.GetLogoAsync(ticker);
            }

        var tickerResultsBrandingDto = new TickerResultsLogoDto(tickerResultsDto, logoDto);
        return Ok(tickerResultsBrandingDto);
    }

    [HttpGet("{ticker}/open-close/{from}")]
    public async Task<IActionResult> GetTickerOpenCloseAsync(string ticker, string from)
    {
        var client = _clientFactory.CreateClient();

        DailyOpenCloseDto? dailyOpenCloseDto;
        try
        {
            dailyOpenCloseDto = await client.GetFromJsonAsync<DailyOpenCloseDto>(
                $"https://api.polygon.io/v1/open-close/{ticker}/{from}?apiKey={_configuration["Polygon:ApiKey"]}");
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

    [HttpGet("{ticker}/aggregates")]
    public async Task<IActionResult> GetAggregatesAsync(string from, string to)
    {
        return null;
    }
}