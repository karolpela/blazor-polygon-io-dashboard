using System.Text.Json.Serialization;
using FinanceApp.Server.Models.TickerDetails;

namespace FinanceApp.Server.Models.News;

public class NewsResult
{
    public NewsResult()
    {
    }

    public NewsResult(string idNews, Publisher publisher, string title, string author, DateTime publishedUtc,
        string articleUrl, byte[] image, string? description, /*List<string>? keywords,*/ string? ampUrl)
    {
        IdNews = idNews;
        Publisher = publisher;
        Title = title;
        Author = author;
        PublishedUtc = publishedUtc;
        ArticleUrl = articleUrl;
        Image = image;
        Description = description;
    }

    public virtual ICollection<TickerResults> TickerResults { get; set; } = new HashSet<TickerResults>();

    public string PublisherName { get; set; }

    [JsonPropertyName("id")] public string IdNews { get; set; }

    [JsonPropertyName("publisher")] public Publisher Publisher { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("author")] public string Author { get; set; }

    [JsonPropertyName("published_utc")] public DateTime PublishedUtc { get; set; }

    [JsonPropertyName("article_url")] public string ArticleUrl { get; set; }

    public byte[]? Image { get; set; }

    [JsonPropertyName("description")] public string? Description { get; set; }
}