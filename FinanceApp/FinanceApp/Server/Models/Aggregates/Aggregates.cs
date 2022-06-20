using System.Text.Json.Serialization;

namespace FinanceApp.Server.Models.Aggregates;

public class Aggregates
{
    [JsonPropertyName("ticker")] public string Ticker { get; set; }

    [JsonPropertyName("queryCount")] public int QueryCount { get; set; }

    [JsonPropertyName("resultsCount")] public int ResultsCount { get; set; }

    [JsonPropertyName("adjusted")] public bool Adjusted { get; set; }

    [JsonPropertyName("results")] public List<AggregateResult> Results { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; }

    [JsonPropertyName("request_id")] public string RequestId { get; set; }

    [JsonPropertyName("count")] public int Count { get; set; }
}