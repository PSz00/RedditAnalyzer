﻿using CSharpFunctionalExtensions;

namespace RedditAnalyzer.Domain.Entities;

public class Comment : Entity<Guid>
{
    public string Text { get; private set; }
    public DateTime? CreatedDate { get; private set; }
    public string Url { get; private set; }
    public Submission? Submission { get; private set; }
    public User? User { get; private set; }

    private Comment(string text, string url, DateTime? createdDate) 
    {
        Url = url;
        Text = text;
        CreatedDate = createdDate;
    }

    public static Comment Create(Submission submission, User user, string text, string url, DateTime? createdDate)
    {
        return new Comment(text, url, createdDate)
        {
            Submission = submission,
            User = user,
        };
    }
}
