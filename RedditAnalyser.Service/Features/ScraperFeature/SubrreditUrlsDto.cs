using RedditAnalyzer.Domain.Entities;

namespace RedditAnalyzer.Service.Features.ScraperFeature;

internal class SubrreditUrlsDto
{
    public Guid Id { get; set; }
    public string Link { get; set; }

    public SubrreditUrlsDto(Url url)
    {
        Id = url.Id;
        Link = url.Text;
    }
}