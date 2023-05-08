using MediatR;

namespace RedditAnalyzer.Service.Infrastructure.CQRS;

internal interface IQuery<out TResult> : IRequest<TResult>
{
}