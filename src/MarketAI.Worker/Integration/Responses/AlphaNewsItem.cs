using System.Text.Json.Serialization;

namespace MarketAI.Worker.Integration.Responses
{
    public class AlphaNewsItem
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        [JsonPropertyName("time_published")]
        public string TimePublished { get; set; } = string.Empty;

        [JsonPropertyName("authors")]
        public List<string> Authors { get; set; } = new List<string>();

        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        [JsonPropertyName("banner_image")]
        public string BannerImage { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("category_within_source")]
        public string CategoryWithinSource { get; set; } = string.Empty;

        [JsonPropertyName("source_domain")]
        public string SourceDomain { get; set; } = string.Empty;

        [JsonPropertyName("topics")]
        public List<Topic> Topics { get; set; } = new List<Topic>();

        [JsonPropertyName("overall_sentiment_score")]
        public double OverallSentimentScore { get; set; }

        [JsonPropertyName("overall_sentiment_label")]
        public string OverallSentimentLabel { get; set; } = string.Empty;

        [JsonPropertyName("ticker_sentiment")]
        public List<TickerSentiment> TickerSentiment { get; set; } = new List<TickerSentiment>();
    }

    public class Topic
    {
        [JsonPropertyName("topic")]
        public string TopicName { get; set; } = string.Empty; // Renamed to avoid conflict with class name

        [JsonPropertyName("relevance_score")]
        public string RelevanceScore { get; set; } = string.Empty; // Kept as string based on JSON
    }

    public class TickerSentiment
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; } = string.Empty;

        [JsonPropertyName("relevance_score")]
        public string RelevanceScore { get; set; } = string.Empty; // Kept as string based on JSON

        [JsonPropertyName("ticker_sentiment_score")]
        public string TickerSentimentScore { get; set; } = string.Empty; // Kept as string based on JSON

        [JsonPropertyName("ticker_sentiment_label")]
        public string TickerSentimentLabel { get; set; } = string.Empty;
    }
} 