using MediatR;
using Microsoft.EntityFrameworkCore;
using RedditAnalyzer.Domain.Entities;
using RedditAnalyzer.Persistence;
using RedditAnalyzer.Service.Infrastructure.CQRS;
using RedditAnalyzer.Service.Scrapers;

namespace RedditAnalyzer.Service.Features.ScraperFeature.Commands;

public class ScrapSubmissionsCommand : ICommand<Guid>
{
    public string SubRedditName { get; set; }

    public ScrapSubmissionsCommand(string subRedditName)
    {
        SubRedditName = subRedditName;
    }
}

internal class ScrapSubmissionsCommandHandler : ICommandHandler<ScrapSubmissionsCommand, Guid>
{
    private readonly RedditAnalyzerDbContext _context;
    private readonly SubmissionScraper _scraper;

    public ScrapSubmissionsCommandHandler(RedditAnalyzerDbContext context, SubmissionScraper submissionScraper)
    {
        _context = context;
        _scraper = submissionScraper;
    }

    public async Task<Guid> Handle(ScrapSubmissionsCommand request, CancellationToken cancellationToken)
    {
        var subreddit = await _context.Subreddits
            .SingleOrDefaultAsync(s => s.Name == request.SubRedditName, cancellationToken);

        if (subreddit is null)
            throw new Exception($"{request.SubRedditName} Does not exist");

        var urls = await _context.Urls
            .Where(url => url.Submission == null && url.Text.Contains($"/r/{request.SubRedditName}/"))
            .Select(url => url.Text)
            .ToListAsync(cancellationToken);

        if (urls is null || urls.Count == 0)
            throw new Exception($"{request.SubRedditName} Does not have submissions to scrap");

        var submissions = _scraper.GetSubmissionsInformation(urls.Take(3).TakeLast(1).ToList());

        await CreateSubmissions(subreddit, submissions, cancellationToken);

        return Guid.NewGuid();
    }

    private async Task<Unit> CreateSubmissions(Subreddit subreddit ,List<SubmissionScraper.SubmissionInfo> submissions, CancellationToken cancellationToken)
    {
        foreach (var newSubmission in submissions)
        {
            var url = _context.Urls.SingleOrDefault(url => url.Text == newSubmission.Url);

            if (url is null)
                throw new Exception($"Url does not exist: {newSubmission.Url}");

            var user = _context.Users.SingleOrDefault(user => user.Username == newSubmission.Author);

            if (user is null)
                user = User.Create(newSubmission.Author);

            var submission = _context.Submissions.SingleOrDefault(sub => sub.Url != null && sub.Url.Text == newSubmission.Url);

            if (submission is null)
                submission = Submission.Create(subreddit, user, url, newSubmission.Title, newSubmission.Date);

            var comments = newSubmission.Comments.Select(comment => CreateComment(submission, comment)).ToList();

            await _context.Comments.AddRangeAsync(comments, cancellationToken);
        }

        return Unit.Value;
    }

    private Comment CreateComment(Submission submission ,SubmissionScraper.CommentInfo comment)
    {
        var commenter = _context.Users.SingleOrDefault(user => user.Username == comment.Author);

        if (commenter is null)
            commenter = User.Create(comment.Author);

        return Comment.Create(submission, commenter, comment.Text, comment.Date);
    }
}