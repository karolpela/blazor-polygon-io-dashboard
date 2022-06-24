namespace FinanceApp.Shared.Models.News;

public class NewsResultImageDto
{
    public NewsResultImageDto(NewsResultDto newsResultDto, byte[]? image)
    {
        NewsResultDto = newsResultDto;
        Image = image;
    }

    public NewsResultDto NewsResultDto { get; set; }
    public byte[]? Image { get; set; }
}