using HtmlAgilityPack;

namespace RedditAnalyzer.Service.Scrapers;

public class SubredditScraper
{
    private readonly SeleniumClient _seleniumClient;
    private const string _baseRedditUrl = "https://www.reddit.com";

    public SubredditScraper() 
    {
        _seleniumClient = new SeleniumClient();
    }

    public List<string> GetOnlyNewestLinks(string subredditName, List<string> alreadyScrapped, int maxLinksCount = 1000, bool shouldEarlyBreak = true)
    {
        if (!shouldEarlyBreak && alreadyScrapped.Count == 0)
            throw new Exception("Don`t use it for scraping old subreddits. It`s only for updating");

        _seleniumClient.NavigateTo(GetSubredditUrl(subredditName));

        var rawLinks = GetAllVisibleLinks(0);

        if (shouldEarlyBreak && ShouldEarlyBreak(rawLinks, alreadyScrapped))
            return ConvertRawLinks(rawLinks, alreadyScrapped);

        while (rawLinks.Count(link => link != null) <= maxLinksCount)
        {
            var loaded = LoadMoreSubmissions(rawLinks);

            if (!loaded)
                break;

            var moreLinks = GetAllVisibleLinks(rawLinks.Count);
            rawLinks.AddRange(moreLinks);

            if (shouldEarlyBreak && ShouldEarlyBreak(rawLinks, alreadyScrapped))
                break;
        }

        return ConvertRawLinks(rawLinks, alreadyScrapped);
    }
    public List<string> GetNNewestLinks(string subredditName, int maxLinksCount = 1000)
    {
        return GetOnlyNewestLinks(subredditName, new(), maxLinksCount, true);
    }

    public List<string> GetOldLinks(string subredditName, List<string> alreadyScrapped, int maxLinksCount)
    {
        if (alreadyScrapped.Count == 0)
            return GetNNewestLinks(subredditName, maxLinksCount);

        _seleniumClient.NavigateTo(GetSubredditUrl(subredditName));

        var links = new List<string>();
        var rawLinksCount = 0;

        while (links.Count <= maxLinksCount)
        {
            var loaded = LoadMoreSubmissions(links);

            if (!loaded)
                break;

            var moreLinks = GetAllVisibleLinks(rawLinksCount);
            var converted = ConvertRawLinks(moreLinks, alreadyScrapped);

            links.AddRange(converted);
            rawLinksCount += moreLinks.Count;
        }

        return links;
    }

    private bool ShouldEarlyBreak(List<string?> rawLinks, List<string> alreadyScrapped) => 
        rawLinks.Any(rawLink => rawLink != null && alreadyScrapped.Contains(rawLink));

    private List<string> ConvertRawLinks(List<string?> rawLinks, List<string> alreadyScrapped) => 
        rawLinks
            .Where(link => link != null)
            .Select(link => link!)
            .Except(alreadyScrapped)
            .ToList();

    private bool LoadMoreSubmissions<T>(List<T> rawLinks)
    {
        _seleniumClient.ScrollToEnd();

        var maxWaitSec = 5;
        var delay = 200;
        var maxTries = delay / maxWaitSec;

        for (var i = 0; i < maxTries; i++)
        {
            Thread.Sleep(delay);
            var links = GetSubmissionsList();

            if (links is null || links.Count == rawLinks.Count) continue;

            return true;
        }

        return false;
    }

    public List<string?> GetAllVisibleLinks(int elementsToSkip = 0)
    {
        var submissions = GetSubmissionsList();

        if (submissions is null)
            return new();

        if (elementsToSkip > 0)
            submissions.RemoveRange(0, elementsToSkip);

        var links = submissions
            .Select(GetSubmissionsUrl)
            .ToList();

        return links ?? new();
    }

    private List<HtmlNode>? GetSubmissionsList()
    {
        var html = _seleniumClient.GetCurrentHtml();

        var firstNode = html.DocumentNode.SelectSingleNode("//div[@data-scroller-first]");

        if (firstNode is null || firstNode.ParentNode is null || firstNode.ParentNode.ChildNodes is null) 
            return null;

        return firstNode.ParentNode.ChildNodes.ToList();
    }

    private string? GetSubmissionsUrl(HtmlNode submission)
    {
        var linkElement = submission.SelectSingleNode(".//a[@data-click-id='body']");

        if (linkElement is null) return null;

        var href = linkElement.Attributes
            .Where(attr => attr.Name == "href")
            .SingleOrDefault();

        if (href == null) return null;

        var test = $"{_baseRedditUrl}{href.Value}";

        return $"{_baseRedditUrl}{href.Value}";
    }

    private string GetSubredditUrl(string subreddit) => $"{_baseRedditUrl}/r/{subreddit}/";

    ~SubredditScraper()
    {
        _seleniumClient.Dispose();
    }
}
