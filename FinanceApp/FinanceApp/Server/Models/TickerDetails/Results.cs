using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Server.Models.TickerDetails
{
    public class Results
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("market")]
        public string Market { get; set; } = null!;

        [JsonPropertyName("locale")]
        public string Locale { get; set; } = null!;

        [JsonPropertyName("primary_exchange")]
        public string PrimaryExchange { get; set; } = null!;

        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("currency_name")]
        public string CurrencyName { get; set; } = null!;

        [JsonPropertyName("cik")]
        public string Cik { get; set; } = null!;

        [JsonPropertyName("composite_figi")]
        public string CompositeFigi { get; set; } = null!;

        [JsonPropertyName("share_class_figi")]
        public string ShareClassFigi { get; set; } = null!;

        [JsonPropertyName("market_cap")]
        public double MarketCap { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; } = null!;

        [JsonPropertyName("address")]
        public Server.Models.TickerDetails.Address Address { get; set; } = null!;

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("sic_code")]
        public string SicCode { get; set; } = null!;

        [JsonPropertyName("sic_description")]
        public string SicDescription { get; set; } = null!;

        [JsonPropertyName("ticker_root")]
        public string TickerRoot { get; set; } = null!;

        [JsonPropertyName("homepage_url")]
        public string HomepageUrl { get; set; } = null!;

        [JsonPropertyName("total_employees")]
        public int TotalEmployees { get; set; }

        [JsonPropertyName("list_date")]
        public string ListDate { get; set; } = null!;

        [JsonPropertyName("branding")]
        public Branding Branding { get; set; } = null!;

        [JsonPropertyName("share_class_shares_outstanding")]
        public long ShareClassSharesOutstanding { get; set; }

        [JsonPropertyName("weighted_shares_outstanding")]
        public long WeightedSharesOutstanding { get; set; }

        public Results()
        {

        }

        public Results(string ticker, string name, string market, string locale, string primaryExchange, string type, bool active, string currencyName, string cik, string compositeFigi, string shareClassFigi, double marketCap, string phoneNumber, Server.Models.TickerDetails.Address address, string description, string sicCode, string sicDescription, string tickerRoot, string homepageUrl, int totalEmployees, string listDate, Branding branding, long shareClassSharesOutstanding, long weightedSharesOutstanding)
        {
            Ticker = ticker;
            Name = name;
            Market = market;
            Locale = locale;
            PrimaryExchange = primaryExchange;
            Type = type;
            Active = active;
            CurrencyName = currencyName;
            Cik = cik;
            CompositeFigi = compositeFigi;
            ShareClassFigi = shareClassFigi;
            MarketCap = marketCap;
            PhoneNumber = phoneNumber;
            Address = address;
            Description = description;
            SicCode = sicCode;
            SicDescription = sicDescription;
            TickerRoot = tickerRoot;
            HomepageUrl = homepageUrl;
            TotalEmployees = totalEmployees;
            ListDate = listDate;
            Branding = branding;
            ShareClassSharesOutstanding = shareClassSharesOutstanding;
            WeightedSharesOutstanding = weightedSharesOutstanding;
        }
    }
}
