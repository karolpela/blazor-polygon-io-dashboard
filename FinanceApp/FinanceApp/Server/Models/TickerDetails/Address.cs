using System.Text.Json.Serialization;

namespace FinanceApp.Server.Models.TickerDetails
{
    public class Address
    {
        [JsonPropertyName("address1")]
        public string Address1 { get; set; } = null!;

        [JsonPropertyName("city")]
        public string City { get; set; } = null!;

        [JsonPropertyName("state")]
        public string State { get; set; } = null!;

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; } = null!;

        public Address()
        {
            
        }

        public Address(string address1, string city, string state, string postalCode)
        {
            Address1 = address1;
            City = city;
            State = state;
            PostalCode = postalCode;
        }
    }
}
