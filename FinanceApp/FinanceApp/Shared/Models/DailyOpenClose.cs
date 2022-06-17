﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FinanceApp.Shared.Models
{
    public class DailyOpenClose
    {
        [JsonPropertyName("status")] public string Status { get; set; }

        [JsonPropertyName("from")] public string From { get; set; }

        [JsonPropertyName("symbol")] public string Symbol { get; set; }

        [JsonPropertyName("open")] public double Open { get; set; }

        [JsonPropertyName("high")] public double High { get; set; }

        [JsonPropertyName("low")] public double Low { get; set; }

        [JsonPropertyName("close")] public double Close { get; set; }

        [JsonPropertyName("volume")] public long Volume { get; set; }

        [JsonPropertyName("afterHours")] public double AfterHours { get; set; }

        [JsonPropertyName("preMarket")] public double PreMarket { get; set; }

        public DailyOpenClose(string status, string from, string symbol, double open, double high, double low,
            double close, long volume, double afterHours, double preMarket)
        {
            Status = status;
            From = from;
            Symbol = symbol;
            Open = open;
            High = high;
            Low = low;
            Close = close;
            Volume = volume;
            AfterHours = afterHours;
            PreMarket = preMarket;
        }
    }
}