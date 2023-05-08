using Microsoft.EntityFrameworkCore;
using RedditAnalyzer.Domain.Entities;
using RedditAnalyzer.Persistence;
using RedditAnalyzer.Service.Infrastructure.CQRS;
using RedditAnalyzer.Service.Scrapers;

namespace RedditAnalyzer.Service.Features.ScraperFeature.Commands;

public class ScrapOldSubredditCommand : ICommand<List<SubrreditUrlsDto>>
{
    public string SubRedditName { get; set; }
    public int NewLinksCount { get; set; }

    public ScrapOldSubredditCommand(string subRedditName, int newLinksCount = 100)
    {
        SubRedditName = subRedditName;
        NewLinksCount = newLinksCount;
    }
}

internal class ScrapOldSubredditCommandHandler : ICommandHandler<ScrapOldSubredditCommand, List<SubrreditUrlsDto>>
{
    private readonly RedditAnalyzerDbContext _context;
    private readonly SubredditScraper _scraper;

    public ScrapOldSubredditCommandHandler(RedditAnalyzerDbContext context, SubredditScraper scraper)
    {
        _context = context;
        _scraper = scraper;
    }

    public async Task<List<SubrreditUrlsDto>> Handle(ScrapOldSubredditCommand request, CancellationToken cancellationToken)
    {
        var subredditUrl = await _context.Subreddits
            .SingleOrDefaultAsync(subreddit => subreddit.Name == request.SubRedditName, cancellationToken);

        if (subredditUrl is null)
            throw new Exception($"Could not find subreddit {request.SubRedditName}");

        var urls = await _context.Urls.ToListAsync(cancellationToken);

        var alreadyScrapped = urls
            .Where(url => url.Text.Contains($"/r/{request.SubRedditName}"))
            .Select(url => url.Text)
            .ToList();

        var links = _scraper.GetOldLinks(request.SubRedditName, alreadyScrapped, 50);

        var newUrls = links.Select(Url.Create).ToList();

        await _context.Urls.AddRangeAsync(newUrls, cancellationToken);

        return newUrls.Select(url => new SubrreditUrlsDto(url)).ToList();
    }
}