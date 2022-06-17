using System.Net;
using FinanceApp.Server.Services;
using FinanceApp.Shared.Models;
using FinanceApp.Shared.Models.TickerDetails;
using FinanceApp.Shared.Models.Tickers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TickersController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IConfiguration _configuration;
    private readonly ITickerDbService _tickerDbService;
    private readonly IUserDbService _userDbService;

    public TickersController(IConfiguration configuration, IHttpClientFactory clientFactory,
        IUserDbService userDbService, ITickerDbService tickerDbService)
    {
        _configuration = configuration;
        _clientFactory = clientFactory;
        _userDbService = userDbService;
        _tickerDbService = tickerDbService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickersAsync()
    {
        var client = _clientFactory.CreateClient();
        var tickers = await client.GetFromJsonAsync<Tickers>(
            "https://api.polygon.io/v3/reference/tickers?type=CS" +
            "&market=stocks&exchange=XNAS&active=true&sort=ticker&order=asc&limit=1000" +
            $"&apiKey={_configuration["Polygon:ApiKey"]}");
        return Ok(tickers);
    }

    [HttpGet("{ticker}")]
    public async Task<IActionResult> GetTickerAsync(string ticker)
    {
        var client = _clientFactory.CreateClient();

        TickerDetailsDto? tickerDetailsDto;

        try
        {
            tickerDetailsDto = await client.GetFromJsonAsync<TickerDetailsDto>(
                $"https://api.polygon.io/v3/reference/tickers/{ticker}?apiKey={_configuration["Polygon:ApiKey"]}");
        }
        catch (HttpRequestException e)
        {
            return new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
        }

        if (tickerDetailsDto == null) return NotFound();

        var logo = Array.Empty<byte>();
        var logoFormat = string.Empty;
        if (tickerDetailsDto.TickerResults.Branding != null)
        {
            try
            {
                logo = await client.GetByteArrayAsync(
                    $"{tickerDetailsDto.TickerResults.Branding.LogoUrl}?apiKey={_configuration["Polygon:ApiKey"]}");
                logoFormat = Path.GetExtension(tickerDetailsDto.TickerResults.Branding.LogoUrl)[1..];
            }
            catch (HttpRequestException e)
            {
                return new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
            }
        }

        var tickerResultsDto = tickerDetailsDto.TickerResults;

        // Save to db
        await _tickerDbService.SaveToDbAsync(tickerResultsDto);


        //var dailyOpenClose = await client.GetFromJsonAsync<DailyOpenClose>(
        //$"https://api.polygon.io/v1/open-close/{ticker}/{DateTime.Now.AddDays(-4):yyyy-MM-dd}?apiKey={_configuration["Polygon:ApiKey"]}");

        var tickerDto = new TickerDto(tickerResultsDto, logo, logoFormat);
        return Ok(tickerDto);
    }

    [HttpGet("{ticker}/open-close")]
    public async Task<IActionResult> GetTickerOpenCloseAsync(string ticker)
    {
        var client = _clientFactory.CreateClient();
        var dailyOpenClose = await client.GetFromJsonAsync<DailyOpenClose>(
            $"https://api.polygon.io/v1/open-close/{ticker}/2022-06-02?apiKey={_configuration["Polygon:ApiKey"]}");
        //var dailyOpenClose = await client.GetFromJsonAsync<DailyOpenClose>(
        //$"https://api.polygon.io/v1/open-close/{ticker}/{DateTime.Now.AddDays(-4):yyyy-MM-dd}?apiKey={_configuration["Polygon:ApiKey"]}");
        return Ok(dailyOpenClose);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> AddTickerToWatchlist(string userId, string ticker)
    {
        var result = await _userDbService.AddToWatchlistAsync(userId, ticker);

        if (result == 0) return BadRequest();

        return Ok(result);
    }
}