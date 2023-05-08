using MediatR;

namespace RedditAnalyzer.Service.Infrastructure.CQRS;

internal interface ICommand : IRequest
{
}

internal interface ICommand<out TResult> : IRequest<TResult>
{
}