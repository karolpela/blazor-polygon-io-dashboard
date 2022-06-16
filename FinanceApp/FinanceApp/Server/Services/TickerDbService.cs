using FinanceApp.Server.Data;
using FinanceApp.Server.Models.TickerDetails;

namespace FinanceApp.Server.Services
{
    public class TickerDbService : ITickerDbService
    {
        private readonly ApplicationDbContext _context;

        public TickerDbService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> SaveToDbAsync(TickerDetails tickerDetails)
        {
            _context.TickerDetails.Update(tickerDetails);
            return await _context.SaveChangesAsync();
        }
    }
}
