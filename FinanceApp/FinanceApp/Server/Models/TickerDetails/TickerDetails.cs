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
        public string TickerId
        {
            get => Results.Ticker;
            set => Results.Ticker = value;
        }

        [JsonPropertyName("results")]
        public Results Results { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("request_id")]
        public string RequestId { get; set; } = null!;

        public ICollection<ApplicationUser> ApplicationUsers { get; set; }

        public TickerDetails()
        {
            ApplicationUsers = new HashSet<ApplicationUser>();
        }

        public TickerDetails(Results results, string status, string requestId)
        {
            Results = results;
            Status = status;
            RequestId = requestId;
            ApplicationUsers = new HashSet<ApplicationUser>();
        }
    }
}
