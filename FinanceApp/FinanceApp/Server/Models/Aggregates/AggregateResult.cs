using System.Text.Json.Serialization;

namespace FinanceApp.Server.Models.Aggregates;

public class AggregateResult
{
    [JsonPropertyName("v")] public int V { get; set; }

    [JsonPropertyName("vw")] public double Vw { get; set; }

    [JsonPropertyName("o")] public double O { get; set; }

    [JsonPropertyName("c")] public double C { get; set; }

    [JsonPropertyName("h")] public double H { get; set; }

    [JsonPropertyName("l")] public double L { get; set; }

    [JsonPropertyName("t")] public ulong T { get; set; }

    [JsonPropertyName("n")] public int N { get; set; }
}