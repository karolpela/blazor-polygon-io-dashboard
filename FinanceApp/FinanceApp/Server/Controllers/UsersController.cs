using FinanceApp.Client.Pages.Stocks;
using FinanceApp.Server.Models.Tickers;
using FinanceApp.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinanceApp.Server.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserDbService _userDbService;

        public UsersController(IUserDbService userDbService)
        {
            _userDbService = userDbService;
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetWatchListAsync(string username)
        {
            return Ok(await _userDbService.GetUserWatchlistAsync(username));
        }

        [HttpGet("{username}/watching/{ticker}")]
        public async Task<IActionResult> IsWatchingAsync(string username, string ticker)
        {
            return await _userDbService.IsOnWatchListAsync(username, ticker) ? NoContent() : NotFound();
        }
    }
}
