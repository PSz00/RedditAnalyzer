using MediatR;
using Microsoft.EntityFrameworkCore;
using RedditAnalyzer.Domain.Entities;
using RedditAnalyzer.Persistence;
using RedditAnalyzer.Service.Infrastructure.CQRS;
using RedditAnalyzer.Service.Infrastructure.Extensions;
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

    public class TestException : Exception
    {

    }

    public async Task<Guid> Handle(ScrapSubmissionsCommand request, CancellationToken cancellationToken)
    {
        var subreddit = await _context.Subreddits
            .SingleOrDefaultAsync(s => s.Name == request.SubRedditName, cancellationToken)
            .ThrowIfNull($"{request.SubRedditName} Does not exist");

        var urls = await _context.Urls
            .Where(url => url.Submission == null && url.Text.Contains($"/r/{request.SubRedditName}/"))
            .Select(url => url.Text)
            .ToListAsync(cancellationToken);


        if (urls is null || urls.Count == 0)
            throw new Exception($"{request.SubRedditName} Does not have submissions to scrap");

        var submissions = _scraper.GetSubmissionsInformation(urls);

        await CreateSubmissions(subreddit, submissions, cancellationToken);

        return Guid.NewGuid();
    }

    private async Task<Unit> CreateSubmissions(Subreddit subreddit, List<SubmissionScraper.SubmissionInfo> submissions, CancellationToken cancellationToken)
    {
        //TODO Move context reads before loop

        foreach (var newSubmission in submissions)
        {
            var url = await _context.Urls
                .SingleOrDefaultAsync(url => url.Text == newSubmission.Url, cancellationToken)
                .ThrowIfNull($"Url does not exist: {newSubmission.Url}");

            var user = _context.Users
                .SingleOrDefault(user => user.Username == newSubmission.Author)
                .IfNull(() => User.Create(newSubmission.Author));

            var submission = _context.Submissions
                .SingleOrDefault(sub => sub.Url != null && sub.Url.Text == newSubmission.Url)
                .IfNull(() => Submission.Create(subreddit, user, url, newSubmission.Title, newSubmission.Date));

            newSubmission.Comments.ForEach(comment => CreateComment(submission, comment));

            var test = await _context.Submissions.AddAsync(submission, cancellationToken);
            var sub = test.Entity;
        }

        return Unit.Value;
    }

    private void CreateComment(Submission submission, SubmissionScraper.CommentInfo comment)
    {
        var commenter = _context.Users.SingleOrDefault(user => user.Username == comment.Author);

        commenter ??= User.Create(comment.Author);

        submission.AddComment(commenter, comment.Text, comment.Url, comment.Date);
    }
}