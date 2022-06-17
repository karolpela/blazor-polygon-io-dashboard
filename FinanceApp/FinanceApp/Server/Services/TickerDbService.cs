using AutoMapper;
using FinanceApp.Server.Data;
using FinanceApp.Server.Models.TickerDetails;
using FinanceApp.Shared.Models.TickerDetails;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Server.Services
{
    public class TickerDbService : ITickerDbService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TickerDbService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> SaveToDbAsync(TickerResultsDto tickerResultsDto)
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
    }
}