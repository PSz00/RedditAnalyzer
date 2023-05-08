using MediatR;
using RedditAnalyzer.Service.Features.ScraperFeature.Commands;

namespace RedditAnalyzer.Api.Controllers;

internal static class ScraperController
{
    private static string BaseUrl => "/api/scrap/";
    public static void AddScraperControllers(this WebApplication app)
    {
        app.MapGet(BaseUrl + "subreddit/add", async (IMediator mediator) => //post
            await mediator.Send(new ScrapNewSubredditCommand("csharp")));

        app.MapGet(BaseUrl + "subreddit/update", async (IMediator mediator) => //post
            await mediator.Send(new ScrapUpdateSubredditCommand("csharp")));

        app.MapGet(BaseUrl + "subreddit/old", async (IMediator mediator) => //post
            await mediator.Send(new ScrapOldSubredditCommand("csharp")));

        app.MapGet(BaseUrl + "submissions", async (IMediator mediator) => //post
            await mediator.Send(new ScrapSubmissionsCommand("csharp")));
    }
}
