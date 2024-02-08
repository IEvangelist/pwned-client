// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Internals;

internal static class AsyncEnumerable
{
    public static IAsyncEnumerable<T> Empty<T>() => EmptyAsyncIterator<T>.Instance;
}

file sealed class EmptyAsyncIterator<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
{
    public static EmptyAsyncIterator<T> Instance { get; } = new();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _) => this;

    public ValueTask<bool> MoveNextAsync() => new(false);

    public T Current => default!;
}
