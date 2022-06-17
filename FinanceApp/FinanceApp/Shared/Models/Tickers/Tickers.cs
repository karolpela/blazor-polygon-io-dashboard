using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Shared.Models.Tickers
{
    public class Tickers
    {
        [JsonPropertyName("results")] public List<Result> Results { get; set; }

        [JsonPropertyName("status")] public string Status { get; set; }

        [JsonPropertyName("request_id")] public string RequestId { get; set; }

        [JsonPropertyName("count")] public int Count { get; set; }

        [JsonPropertyName("next_url")] public string NextUrl { get; set; }

        public Tickers(List<Result> results, string status, string requestId, int count, string nextUrl)
        {
            Results = results;
            Status = status;
            RequestId = requestId;
            Count = count;
            NextUrl = nextUrl;
        }
    }
}