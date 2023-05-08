using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedditAnalyzer.Domain.Entities;

namespace RedditAnalyzer.Persistence.Configuration;

internal class UrlMap : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        builder.HasKey(url => url.Id);
    }
}
