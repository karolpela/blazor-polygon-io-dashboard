﻿using Duende.IdentityServer.Extensions;
using FinanceApp.Server.Services.Interfaces;
using FinanceApp.Shared.Models;
using FinanceApp.Shared.Models.News;
using FinanceApp.Shared.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Server.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TickersController : ControllerBase
{
    private readonly IStockApiService _stockApiService;
    private readonly ITickerDbService _tickerDbService;

    public TickersController(ITickerDbService tickerDbService, IStockApiService stockApiService)
    {
        _tickerDbService = tickerDbService;
        _stockApiService = stockApiService;
    }

    [HttpGet]
    public async Task<IActionResult> GetTickersAsync()
    {
        List<TickerListItemDto> itemList = new();

        // Try to get new data from Polygon
        try
        {
            var tickerListDto = await _stockApiService.GetTickerList();
            if (tickerListDto == null) return NotFound();

            itemList = tickerListDto.Results;
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
        TickerResultsDto? tickerResultsDto;

        try
        {
            //throw new HttpRequestException();
            var tickerDetailsDto = await _stockApiService.GetTickerDetails(ticker);
            if (tickerDetailsDto == null) return NotFound();

            tickerResultsDto = tickerDetailsDto.TickerResults;
            await _tickerDbService.SaveResultsToDbAsync(tickerResultsDto);
        }
        catch (HttpRequestException)
        {
            tickerResultsDto = await _tickerDbService.GetTickerResultsAsync(ticker);
        }

        LogoDto? logoDto;

        if (tickerResultsDto == null)
        {
            // unable to get new results but check db for existing logo
            logoDto = await _tickerDbService.GetLogoAsync(ticker);
        }

        else
        {
            try
            {
                // try to get updated logo
                logoDto = await _stockApiService.GetLogoAsync(tickerResultsDto);
                if (logoDto != null)
                    // save logo to db
                    await _tickerDbService.UpdateLogoAsync(logoDto);
            }
            catch (HttpRequestException)
            {
                // unable to get updated logo
                logoDto = await _tickerDbService.GetLogoAsync(ticker);
            }
        }
            
        if (tickerResultsDto == null && logoDto == null) return NotFound();

        var tickerResultsLogoDto = new TickerResultsLogoDto(tickerResultsDto, logoDto);
        return Ok(tickerResultsLogoDto);
    }

    [HttpGet("{ticker}/open-close/{from}")]
    public async Task<IActionResult> GetTickerOpenCloseAsync(string ticker, string from)
    {
        DailyOpenCloseDto? dailyOpenCloseDto;
        try
        {
            dailyOpenCloseDto = await _stockApiService.GetDailyOpenCloseAsync(ticker, from);
            if (dailyOpenCloseDto != null)
                await _tickerDbService.SaveDailyOpenCloseToDbAsync(ticker, dailyOpenCloseDto);
        }
        catch (HttpRequestException)
        {
            dailyOpenCloseDto = await _tickerDbService.GetDailyOpenCloseAsync(ticker, from);
        }

        return dailyOpenCloseDto != null ? Ok(dailyOpenCloseDto) : NotFound();
    }

    [HttpPost("{ticker}/users")]
    public async Task<IActionResult> SubscribeToTicker(string ticker, [FromBody] string username)
    {
        var result = await _tickerDbService.SubscribeToTickerAsync(ticker, username);

        return result == 0 ? BadRequest() : Ok(result);
    }

    [HttpDelete("{ticker}/users/{username}")]
    public async Task<IActionResult> UnsubscribeFromTicker(string ticker, string username)
    {
        var result = await _tickerDbService.UnsubscribeFromTickerAsync(ticker, username);

        return result == 0 ? BadRequest() : Ok(result);
    }

    [HttpGet("{ticker}/bars")]
    public async Task<IActionResult> GetBarsAsync(string ticker, string timespan, int multiplier,
        long fromUnix, long toUnix)
    {
        var fromOffset = DateTimeOffset.FromUnixTimeMilliseconds(fromUnix);
        var toOffset = DateTimeOffset.FromUnixTimeMilliseconds(toUnix);

        // exchange opens at 1:30 PM UTC and closes at 9:30 PM UTC
        var fromOffsetAdjusted =
            new DateTimeOffset(fromOffset.Year, fromOffset.Month, fromOffset.Day, 13, 30, 0, TimeSpan.Zero);
        var toOffsetAdjusted =
            new DateTimeOffset(toOffset.Year, toOffset.Month, toOffset.Day, 19, 30, 0, TimeSpan.Zero);

        var fromOffsetAdjustedUnix = fromOffsetAdjusted.ToUnixTimeMilliseconds();
        var toOffsetAdjustedUnix = toOffsetAdjusted.ToUnixTimeMilliseconds();

        IEnumerable<StockChartDataDto>? chartDataDtoList;
        try
        {
            chartDataDtoList = await _stockApiService.GetChartData(ticker, timespan, multiplier,
                fromOffsetAdjustedUnix, toOffsetAdjustedUnix);
            // save to db
            await _tickerDbService.SaveStockChartDataAsync(chartDataDtoList, ticker, timespan, multiplier,
                DateTime.Now.Date);
        }
        catch (HttpRequestException)
        {
            // get from db
            chartDataDtoList = (await _tickerDbService.GetStockChartDataAsync(ticker, timespan, multiplier,
                DateTime.Now.Date, fromOffsetAdjusted.DateTime, toOffsetAdjusted.DateTime)).ToList();
            if (chartDataDtoList.IsNullOrEmpty()) return NotFound();
            return Ok(chartDataDtoList);
        }

        return Ok(chartDataDtoList);
    }

    [HttpGet("{ticker}/news")]
    public async Task<IActionResult> GetNewsAsync(string ticker, int count)
    {
        IEnumerable<NewsResultImageDto>? resultsImagesDtoList;
        try
        {
            resultsImagesDtoList = await _stockApiService.GetNewsAsync(ticker, count);
            await _tickerDbService.SaveNewsImagesAsync(resultsImagesDtoList);
        }
        catch (HttpRequestException)
        {
            // get from db
            Console.WriteLine("Unable to get news from Polygon - getting from database instead");
            var newsImagesDtos = await _tickerDbService.GetNewsImagesAsync(ticker, count);
            resultsImagesDtoList = new List<NewsResultImageDto>(newsImagesDtos);
        }

        return Ok(resultsImagesDtoList);
    }
}