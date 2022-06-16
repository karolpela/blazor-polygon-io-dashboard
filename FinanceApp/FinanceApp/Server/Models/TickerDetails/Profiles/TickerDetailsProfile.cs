using AutoMapper;
using FinanceApp.Shared.Models.TickerDetails;

namespace FinanceApp.Server.Models.TickerDetails.Profiles
{
    public class TickerDetailsProfile : Profile
    {
        public TickerDetailsProfile()
        {
            CreateMap<TickerDetails, TickerDetailsDto>();
        }
    }
}
