﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Server.Models.TickerDetails
{
    public class Branding
    {
        [JsonPropertyName("logo_url")] public string LogoUrl { get; set; } = null!;

        [JsonPropertyName("icon_url")] public string? IconUrl { get; set; }

        public Branding()
        {
        }

        public Branding(string logoUrl, string iconUrl)
        {
            LogoUrl = logoUrl;
            IconUrl = iconUrl;
        }
    }
}