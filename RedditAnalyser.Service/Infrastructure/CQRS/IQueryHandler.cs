using MediatR;

namespace RedditAnalyzer.Service.Infrastructure.CQRS;

internal interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
}