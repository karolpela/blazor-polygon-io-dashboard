using AutoMapper;
using FinanceApp.Shared.Models;
using FinanceApp.Shared.Models.TickerDetails;
using FinanceApp.Shared.Models.Tickers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TickersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IMapper _mapper;

        public TickersController(IConfiguration configuration, IHttpClientFactory clientFactory, IMapper mapper)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickersAsync()
        {
            var client = _clientFactory.CreateClient();
            var tickers = await client.GetFromJsonAsync<Tickers>($"https://api.polygon.io/v3/reference/tickers" +
                $"?type=CS&market=stocks&exchange=XNAS&active=true&sort=ticker&order=asc&limit=1000" +
                $"&apiKey={_configuration["Polygon:ApiKey"]}");
            return Ok(tickers);
        }

        [HttpGet("{ticker}")]
        public async Task<IActionResult> GetTickerDetailsAsync(string ticker)
        {

            var client = _clientFactory.CreateClient();
            var tickerDetails = await client.GetFromJsonAsync<TickerDetailsDto>(
                $"https://api.polygon.io/v3/reference/tickers/{ticker}?apiKey={_configuration["Polygon:ApiKey"]}");
            var tickerDetailsDto = _mapper.Map<TickerDetailsDto>(tickerDetails);
            return Ok(tickerDetailsDto);
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

        [HttpGet("{ticker}/logo")]
        public async Task<IActionResult> GetTickerLogo(string ticker)
        {
            var client = _clientFactory.CreateClient();
            var logo = await client.GetByteArrayAsync("https://api.polygon.io/v1/reference/company-branding/d3d3LmFwcGxlLmNvbQ/images/2022-05-01_logo.svg?apiKey=WEKFwLl3oNRV5JLtyQfWN9HcbtttQTJL");
            return File(logo, "image/svg+xml");
        }
    }
}
