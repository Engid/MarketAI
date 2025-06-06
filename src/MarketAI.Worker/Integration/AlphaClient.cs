﻿using MarketAI.Worker.Integration.Responses;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Text.Json;

namespace MarketReportAI.AIClient;

public interface IMarketDataClient
{
    Task<DailyQuote[]> GetDailyQuotesAsync(string symbol, DateTime from, DateTime to);

    Task<AlphaNewsItem[]> GetNews(string symbol);
}

public class AlphaVantageClient : IMarketDataClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public AlphaVantageClient(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["AlphaVantage:ApiKey"] ?? throw new ArgumentException("AlphaVantage ApiKey required");
    }

    public async Task<AlphaNewsItem[]> GetNews(string symbol)
    {
        var url = $"query?function=NEWS_SENTIMENT&tickers={symbol}&apikey={_apiKey}";

        // Make the HTTP request
        var response = await _http.GetAsync(url);
        response.EnsureSuccessStatusCode();

        // Parse the JSON response into AlphaNewsItem[]
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        if (json.TryGetProperty("feed", out var feedJson))
        {
            var newsItems = JsonSerializer.Deserialize<AlphaNewsItem[]>(feedJson.GetRawText());
            return newsItems ?? Array.Empty<AlphaNewsItem>();
        }

        return Array.Empty<AlphaNewsItem>();
    }

    // TODO:
    public async Task<DailyQuote[]> GetDailyQuotesAsync(string symbol, DateTime from, DateTime to)
    {
        var url = $"query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={_apiKey}&outputsize=full";
        var json = await _http.GetFromJsonAsync<JsonElement>(url);
        // parse json["Time Series (Daily)"] into DailyQuote[]
        return [new()];
    }
}

// TODO:
public record DailyQuote();

