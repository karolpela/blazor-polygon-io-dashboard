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
        [JsonPropertyName("resultsDto")]
        public ResultsDto ResultsDto { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; } = null!;


        public TickerDetailsDto()
        {
        }

        public TickerDetailsDto(ResultsDto resultsDto, string status, string requestId)
        {
            ResultsDto = resultsDto;
            Status = status;
            RequestId = requestId;
        }
    }
}
