namespace FinanceApp.Server.Models.News;

public class NewsImage
{
#pragma warning disable CS8618
    public NewsImage()
#pragma warning restore CS8618
    {
    }

    public NewsImage(byte[] data, string? format, string idNews)
    {
        Data = data;
        Format = format;
        IdNews = idNews;
    }

    public virtual NewsResult NewsResult { get; set; } = null!;
    public string IdNews { get; set; }
    public byte[] Data { get; set; }
    public string? Format { get; set; }
}