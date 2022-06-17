using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceApp.Shared.Models.TickerDetails
{
    public class TickerDto
    {
        public TickerResultsDto TickerResultsDto { get; set; }
        public byte[]? Logo { get; set; }

        public string? LogoFormat
        {
            get => _logoFormat == "svg" ? "svg+xml" : _logoFormat;
            set => _logoFormat = value;
        }

        private string? _logoFormat;
        public TickerDto(TickerResultsDto tickerResultsDto, byte[] logo, string logoFormat)
        {
            TickerResultsDto = tickerResultsDto;
            Logo = logo;
            LogoFormat = logoFormat;
        }
    }
}