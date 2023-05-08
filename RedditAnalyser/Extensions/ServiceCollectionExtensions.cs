using RedditAnalyzer.Persistence;
using Microsoft.EntityFrameworkCore;

namespace RedditAnalyzer.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RedditAnalyzerDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("RedditAnalyzerDatabase")));
    }
}

