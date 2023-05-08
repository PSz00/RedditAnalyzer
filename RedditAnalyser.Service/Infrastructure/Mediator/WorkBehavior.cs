using MediatR;
using RedditAnalyzer.Persistence;
using RedditAnalyzer.Service.Infrastructure.CQRS;

namespace RedditAnalyzer.Service.Infrastructure.Mediator;

public class WorkBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    private readonly RedditAnalyzerDbContext _context;

    public WorkBehavior(RedditAnalyzerDbContext context)
    {
        _context = context;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var result = await next();

        if (request is ICommand || request is ICommand<TResponse>)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        return result;
    }
}