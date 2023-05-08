using CSharpFunctionalExtensions;

namespace RedditAnalyzer.Domain.Entities;
 
public class Subreddit : Entity<Guid>
{
    private readonly List<Submission> _submissions = new ();

    public string Name { get; private set; }
    public string Url => $"https://www.reddit.com/r/{Name}";
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();

    private Subreddit(string name)
    {
        Name = name;
    }

    public static Subreddit Create(string name)
    {
        return new Subreddit(name);
    }

    public Subreddit AddSubmission(Submission submission)
    {
        _submissions.Add(submission);
        return this;
    }

    public Subreddit AddSubmissions(IEnumerable<Submission> submissions)
    {
        _submissions.AddRange(submissions);
        return this;
    }
}
