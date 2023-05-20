using System.Diagnostics.CodeAnalysis;

namespace RedditAnalyzer.Service.Infrastructure.Extensions;

public static class LinqExtensions
{
    public static IEnumerable<T> ThrowIf<T>(this IEnumerable<T> source, Predicate<T> predicate, string? message = null)
    {
        foreach (T item in source)
        {
            if (predicate(item))
                yield return item;

            throw new Exception(message);
        }
    }

    public static IEnumerable<T> ThrowIf<T, TException>(this IEnumerable<T> source, Predicate<T> predicate, string? message = null)
        where TException : Exception, new()
    {
        foreach (T item in source)
        {
            if (!predicate(item))
                yield return item;

            throw ThrowCustomClass<TException>(message);
        }
    }

    public static T ThrowIf<T>(this T item, Predicate<T> predicate, string? message = null)
    {
        if (!predicate(item))
            return item;

        throw new Exception(message);
    }

    public static async Task<T> ThrowIf<T>(this Task<T> itemTask, Predicate<T> predicate, string? message = null, CancellationToken? cancellationToken = null)
    {
        T? item = await itemTask.ConfigureAwait(false);

        cancellationToken?.ThrowIfCancellationRequested();

        if (!predicate(item))
            return item;

        throw new Exception(message);
    }

    public static T ThrowIf<T, TException>(this T item, Predicate<T> predicate, string? message = null)
        where TException : Exception, new()
    {
        if (!predicate(item))
            return item;

        throw ThrowCustomClass<TException>(message);
    }


    [return: NotNull]
    public static T ThrowIfNull<T>(this T? item, string? message = null)
    {
        if (item is not null)
            return item;

        throw new Exception(message);
    }

    [return: NotNull]
    public static async Task<T> ThrowIfNull<T>(this Task<T?> itemTask, string? message = null)
    {
        T? item = await itemTask.ConfigureAwait(false);

        if (item is not null)
            return item;

        throw new Exception(message);
    }

    [return: NotNull]
    public static T ThrowIfNull<T, TException>(this T? item, string? message = null)
        where TException : Exception, new()
    {
        if (item is not null)
            return item;

        throw ThrowCustomClass<TException>(message);
    }

    [return: NotNull]
    public static async Task<T> ThrowIfNull<T, TException>(this Task<T?> itemTask, string? message = null)
        where TException : Exception, new()
    {
        T? item = await itemTask.ConfigureAwait(false);

        if (item is not null)
            return item;

        throw ThrowCustomClass<TException>(message);
    }

    [return: NotNull]
    public static T IfNull<T>(this T item, Func<T> function)
    {
        if (item is not null)
            return item;

        var newItem = function();

        if (newItem is not null)
            return newItem;

        throw new Exception($"New item is null: {nameof(function)}");
    }

    private static Exception ThrowCustomClass<TException>(string? message = null) where TException : Exception, new()
    {
        var exception = Activator
            .CreateInstance(typeof(TException), message) as Exception;

        if (exception is null)
            throw new Exception($"Could not create Exception from class {typeof(TException)}");

        return exception;
    }
}
