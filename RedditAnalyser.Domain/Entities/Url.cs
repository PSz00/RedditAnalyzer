using CSharpFunctionalExtensions;

namespace RedditAnalyzer.Domain.Entities;

public class Url : Entity<Guid>
{
    public string Text { get; private set; }
    public Submission? Submission { get; private set; }

    private Url(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentNullException(nameof(text));

        Text = text;
    }

    public static Url Create(string text)
    {
        return new Url(text);
    }
}
