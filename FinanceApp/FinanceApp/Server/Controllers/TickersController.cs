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

        //TODO organize and send from db if Polygon is unavailable
        try
        {
            tickerDetailsDto = await client.GetFromJsonAsync<TickerDetailsDto>(
                $"https://api.polygon.io/v3/reference/tickers/{ticker}?apiKey={_configuration["Polygon:ApiKey"]}");
        }
        catch (HttpRequestException e)
        {
            return HandleHttpRequestException(e);
        }

        if (tickerDetailsDto == null) return NotFound();

        var logo = Array.Empty<byte>();
        var logoFormat = string.Empty;
        if (tickerDetailsDto.TickerResults.Branding != null)
            if (tickerDetailsDto.TickerResults.Branding.IconUrl != null)
            {
                try
                {
                    logo = await client.GetByteArrayAsync(
                        $"{tickerDetailsDto.TickerResults.Branding.LogoUrl}?apiKey={_configuration["Polygon:ApiKey"]}");
                    logoFormat = Path.GetExtension(tickerDetailsDto.TickerResults.Branding.LogoUrl)[1..];
                }
                catch (HttpRequestException e)
                {
                    return HandleHttpRequestException(e);
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

    private static IActionResult HandleHttpRequestException(HttpRequestException e)
    {
        if (e.StatusCode == HttpStatusCode.TooManyRequests)
            return new StatusCodeResult((int)HttpStatusCode.TooManyRequests);
        return new StatusCodeResult((int)HttpStatusCode.BadRequest);
    }

    [HttpGet("{ticker}/open-close")]
    public async Task<IActionResult> GetTickerOpenCloseAsync(string ticker)
    {
        var client = _clientFactory.CreateClient();

        DailyOpenClose? dailyOpenClose = null;
        try
        {
            dailyOpenClose = await client.GetFromJsonAsync<DailyOpenClose>(
                $"https://api.polygon.io/v1/open-close/{ticker}/2022-06-02?apiKey={_configuration["Polygon:ApiKey"]}");
        }
        catch (HttpRequestException e)
        {
            return HandleHttpRequestException(e);
        }

        //var dailyOpenClose = await client.GetFromJsonAsync<DailyOpenClose>(
        //$"https://api.polygon.io/v1/open-close/{ticker}/{DateTime.Now.AddDays(-4):yyyy-MM-dd}?apiKey={_configuration["Polygon:ApiKey"]}");
        return dailyOpenClose != null ? Ok(dailyOpenClose) : NotFound();
    }

    [AllowAnonymous]
    [HttpPost("{ticker}/users")]
    public async Task<IActionResult> SubscribeToTicker(string ticker, [FromBody]string username)
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

}