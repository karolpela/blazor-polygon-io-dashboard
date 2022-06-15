using AutoMapper;
using FinanceApp.Shared.Models.TickerDetails;

namespace FinanceApp.Server.Models.TickerDetails
{
    public class TickerDetailsProfile : Profile
    {
        public TickerDetailsProfile()
        {
            CreateMap<TickerDetails, TickerDetailsDto>();
        }
    }
}
