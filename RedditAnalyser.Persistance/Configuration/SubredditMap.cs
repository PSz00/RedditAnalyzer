using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedditAnalyzer.Domain.Entities;

namespace RedditAnalyzer.Persistence.Configuration;

internal class SubredditMap : IEntityTypeConfiguration<Subreddit>
{
    public void Configure(EntityTypeBuilder<Subreddit> builder)
    {
        builder.HasKey(subreddit => subreddit.Id);
    }
}
