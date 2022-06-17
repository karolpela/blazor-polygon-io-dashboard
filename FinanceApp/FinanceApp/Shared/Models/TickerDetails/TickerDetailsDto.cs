using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Shared.Models.TickerDetails
{
    public class TickerDetailsDto
    {
        [JsonPropertyName("results")] public TickerResultsDto TickerResults { get; set; } = null!;

        [JsonPropertyName("status")] public string Status { get; set; } = null!;

        [JsonPropertyName("request_id")] public string RequestId { get; set; } = null!;


        public TickerDetailsDto()
        {
        }

        public TickerDetailsDto(TickerResultsDto tickerResultsDto, string status, string requestId)
        {
            TickerResults = tickerResultsDto;
            Status = status;
            RequestId = requestId;
        }
    }
}