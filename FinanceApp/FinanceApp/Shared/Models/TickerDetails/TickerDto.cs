using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Shared.Models.TickerDetails
{
    public class TickerDto
    {
        public TickerDetailsDto TickerDetailsDto{ get; set; }
        public byte[] Logo { get; set; }

        public TickerDto(TickerDetailsDto tickerDetailsDto, byte[] logo)
        {
            TickerDetailsDto = tickerDetailsDto;
            Logo = logo;

        }
    }
}
