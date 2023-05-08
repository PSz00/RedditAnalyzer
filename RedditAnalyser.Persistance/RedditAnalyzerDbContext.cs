using Microsoft.EntityFrameworkCore;
using RedditAnalyzer.Domain.Entities;
using RedditAnalyzer.Persistence.Configuration;

namespace RedditAnalyzer.Persistence;

public class RedditAnalyzerDbContext : DbContext
{
    public RedditAnalyzerDbContext(DbContextOptions<RedditAnalyzerDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UrlMap());
        modelBuilder.ApplyConfiguration(new UserMap());
        modelBuilder.ApplyConfiguration(new CommentMap());
        modelBuilder.ApplyConfiguration(new SubredditMap());
        modelBuilder.ApplyConfiguration(new SubmissionMap());
    }

    public DbSet<Url> Urls { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Subreddit> Subreddits { get; set; }
    public DbSet<Submission> Submissions { get; set; }
}