using Microsoft.EntityFrameworkCore;
using RedditAnalyzer.Domain.Entities;
using RedditAnalyzer.Persistence;
using RedditAnalyzer.Service.Infrastructure.CQRS;
using RedditAnalyzer.Service.Scrapers;

namespace RedditAnalyzer.Service.Features.ScraperFeature.Commands;

public class ScrapNewSubredditCommand : ICommand<List<SubrreditUrlsDto>>
{
    public string SubRedditName { get; set; }
    public int NewLinksCount { get; set; }

    public ScrapNewSubredditCommand(string subRedditName, int newLinksCount = 100)
    {
        SubRedditName = subRedditName;
        NewLinksCount = newLinksCount;
    }
}

internal class ScrapNewSubredditCommandHandler : ICommandHandler<ScrapNewSubredditCommand, List<SubrreditUrlsDto>>
{
    private readonly RedditAnalyzerDbContext _context;
    private readonly SubredditScraper _scraper;

    public ScrapNewSubredditCommandHandler(RedditAnalyzerDbContext context, SubredditScraper scraper)
    {
        _context = context;
        _scraper = scraper;
    }

    public async Task<List<SubrreditUrlsDto>> Handle(ScrapNewSubredditCommand request, CancellationToken cancellationToken)
    {
        var subredditUrl = await _context.Urls
            .SingleOrDefaultAsync(url => url.Text == $"https://www.reddit.com/r/{request.SubRedditName}", cancellationToken);

        if (subredditUrl is not null)
            throw new Exception($"{request.SubRedditName} already exists");

        var newSubreddit = Subreddit.Create(request.SubRedditName);

        await _context.Subreddits.AddAsync(newSubreddit, cancellationToken);

        var links = _scraper.GetNNewestLinks(request.SubRedditName, 50);

        var newUrls = links.Select(Url.Create).ToList();

        await _context.Urls.AddRangeAsync(newUrls, cancellationToken);

        return newUrls.Select(url => new SubrreditUrlsDto(url)).ToList();
    }
}