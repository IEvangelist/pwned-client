// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Internals;

internal static class AsyncEnumerable
{
    public static IAsyncEnumerable<T> Empty<T>() => EmptyAsyncInterator<T>.Instance;
}

file sealed class EmptyAsyncInterator<T> : IAsyncEnumerable<T>, IAsyncEnumerator<T>
{
    public static EmptyAsyncInterator<T> Instance { get; } = new();

    public ValueTask DisposeAsync() => default;

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken _) => this;

    public ValueTask<bool> MoveNextAsync() => new(false);

    public T Current => default!;
}
