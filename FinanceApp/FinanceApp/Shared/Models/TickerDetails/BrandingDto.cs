using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Shared.Models.TickerDetails
{
    public class BrandingDto
    {
        [JsonPropertyName("logo_url")]
        public string LogoUrl { get; set; }

        [JsonPropertyName("icon_url")]
        public string IconUrl { get; set; }

        public BrandingDto(string logoUrl, string iconUrl)
        {
            LogoUrl = logoUrl;
            IconUrl = iconUrl;
        }
    }
}
