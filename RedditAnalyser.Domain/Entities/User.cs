using CSharpFunctionalExtensions;

namespace RedditAnalyzer.Domain.Entities;

public class User : Entity<Guid>
{
    private List<Comment> _comments = new();
    private List<Submission> _submissions = new();

    public string Username { get; private set; }
    public string Url => $"https://www.reddit.com/user/{Username}";
    public IReadOnlyCollection<Submission> Submissions => _submissions.AsReadOnly();
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private User(string username)
    {
        Username = username;
    }
    
    public static User Create(string username)
    {
        var user = new User(username);

        return user;
    }
}
