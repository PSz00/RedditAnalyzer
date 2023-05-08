using Microsoft.EntityFrameworkCore;
using RedditAnalyzer.Domain.Entities;
using RedditAnalyzer.Persistence;
using RedditAnalyzer.Service.Infrastructure.CQRS;
using RedditAnalyzer.Service.Scrapers;

namespace RedditAnalyzer.Service.Features.ScraperFeature.Commands;

public class ScrapUpdateSubredditCommand : ICommand<List<SubrreditUrlsDto>>
{
    public string SubRedditName { get; set; }
    public int NewLinksCount { get; set; }

    public ScrapUpdateSubredditCommand(string subRedditName, int newLinksCount = 100)
    {
        SubRedditName = subRedditName;
        NewLinksCount = newLinksCount;
    }
}

internal class ScrapUpdateSubredditCommandHandler : ICommandHandler<ScrapUpdateSubredditCommand, List<SubrreditUrlsDto>>
{
    private readonly RedditAnalyzerDbContext _context;
    private readonly SubredditScraper _scraper;

    public ScrapUpdateSubredditCommandHandler(RedditAnalyzerDbContext context, SubredditScraper scraper)
    {
        _context = context;
        _scraper = scraper;
    }

    public async Task<List<SubrreditUrlsDto>> Handle(ScrapUpdateSubredditCommand request, CancellationToken cancellationToken)
    {
        var subredditUrl = await _context.Urls
            .SingleOrDefaultAsync(url => url.Text == $"https://www.reddit.com/r/{request.SubRedditName}", cancellationToken);

        if (subredditUrl is null)
            throw new Exception($"Could not find subreddit {request.SubRedditName}");
    
        var urls = await _context.Urls.ToListAsync(cancellationToken);

        var alreadyScrapped = urls
            .Where(url => url.Text.Contains($"/r/{request.SubRedditName}"))
            .Select(url => url.Text)
            .ToList();

        var links = _scraper.GetOnlyNewestLinks(request.SubRedditName, alreadyScrapped);

        var newUrls = links.Select(Url.Create).ToList();

        await _context.Urls.AddRangeAsync(newUrls, cancellationToken);

        return newUrls.Select(url => new SubrreditUrlsDto(url)).ToList();
    }
}