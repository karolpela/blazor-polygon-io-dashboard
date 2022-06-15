using System.Text.Json.Serialization;

namespace FinanceApp.Server.Models.TickerDetails
{
    public class Address
    {
        [JsonPropertyName("address1")]
        public string Address1 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("postal_code")]
        public string PostalCode { get; set; }

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
