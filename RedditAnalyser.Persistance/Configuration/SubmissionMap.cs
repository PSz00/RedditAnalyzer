using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedditAnalyzer.Domain.Entities;

namespace RedditAnalyzer.Persistence.Configuration;

internal class SubmissionMap : IEntityTypeConfiguration<Submission>
{
    public void Configure(EntityTypeBuilder<Submission> builder)
    {
        builder.HasKey(submission => submission.Id);

        builder
            .HasOne(submission => submission.Subreddit)
            .WithMany(reddit => reddit.Submissions)
            .IsRequired();

        builder
            .HasOne(submission => submission.Url)
            .WithOne(url => url.Submission)
            .HasForeignKey<Submission>(submission => submission.UrlId)
            .IsRequired();

        builder
            .HasOne(submission => submission.Creator)
            .WithMany(user => user.Submissions)
            .HasForeignKey(submission => submission.Id)
            .IsRequired();
    }
}