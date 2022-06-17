using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Server.Models.TickerDetails
{
    public sealed class TickerDetails
    {
        public string TickerId;

        private TickerResults _tickerResults = null!;

        [JsonPropertyName("results")]
        public TickerResults TickerResults
        {
            get => _tickerResults;
            set
            {
                _tickerResults = value;
                TickerId = value.Ticker;
            }
        }

        [JsonPropertyName("status")] public string Status { get; set; } = null!;

        [JsonPropertyName("request_id")] public string RequestId { get; set; } = null!;

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public TickerDetails()
        {
            TickerId = string.Empty;
            ApplicationUsers = new HashSet<ApplicationUser>();
        }

        public TickerDetails(TickerResults tickerResults, string status, string requestId)
        {
            TickerId = tickerResults.Ticker;
            TickerResults = tickerResults;
            Status = status;
            RequestId = requestId;
            ApplicationUsers = new HashSet<ApplicationUser>();
        }
    }
}