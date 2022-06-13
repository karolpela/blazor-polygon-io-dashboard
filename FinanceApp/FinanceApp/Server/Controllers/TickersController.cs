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

        public TickersController(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
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
            var tickerDetails = await client.GetFromJsonAsync<TickerDetails>(
                $"https://api.polygon.io/v3/reference/tickers/{ticker}?apiKey={_configuration["Polygon:ApiKey"]}");
            return Ok(tickerDetails);
        }
    }
}
