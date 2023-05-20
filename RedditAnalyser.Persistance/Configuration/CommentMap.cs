using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedditAnalyzer.Domain.Entities;

namespace RedditAnalyzer.Persistence.Configuration;

internal class CommentMap : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(comment => comment.Id);

        builder
            .HasOne(comment => comment.User)
            .WithMany(user => user.Comments)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired(true);

        builder
            .HasOne(comment => comment.Submission)
            .WithMany(submission => submission.Comments)
            .IsRequired(true);
    }
}
