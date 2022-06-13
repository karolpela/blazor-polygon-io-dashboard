using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Shared.Models.TickerDetails
{
    public class TickerDetails
    {
        [JsonPropertyName("results")]
        public Results Results { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        public TickerDetails(Results results, string status, string requestId)
        {
            Results = results;
            Status = status;
            RequestId = requestId;
        }
    }
}
