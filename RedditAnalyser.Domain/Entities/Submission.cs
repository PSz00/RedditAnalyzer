using CSharpFunctionalExtensions;

namespace RedditAnalyzer.Domain.Entities;

public class Submission : Entity<Guid>
{
    private List<Comment> _comments = new();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();
    public string Title { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public bool IsDead { get; private set; }
    public Subreddit? Subreddit { get; private set; }
    public Guid UrlId { get; private set; }
    public Url? Url { get; private set; }
    public User? Creator { get; private set; }


    private Submission(string title, DateTime createdDate)
    {
        Title = title;
        CreatedDate = createdDate;
        IsDead = false;
    }

    public static Submission Create(Subreddit subreddit, User creator, Url url, string title, DateTime createdDate)
    {
        var submission = new Submission(title, createdDate)
        {
            Subreddit = subreddit,
            UrlId = url.Id,
            Url = url,
            Creator = creator,
        };

        return submission;
    }

    public void AddComment(User user, string text, string url, DateTime? createdDate)
    {
        _comments.Add(Comment.Create(this, user, text, url, createdDate));
    }
}
