using MediatR;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RedditAnalyzer.Service.Scrapers;
using RedditAnalyzer.Service.Infrastructure.Mediator;

namespace RedditAnalyzer.Service;

public static class Dependencies
{
    public static void AddServicesDependency(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(WorkBehavior<,>));

        services.AddScrappers();
    }

    private static void AddScrappers(this IServiceCollection services)
    {
        services.AddScoped<SubredditScraper>();
        services.AddScoped<SubmissionScraper>();
    }
}
