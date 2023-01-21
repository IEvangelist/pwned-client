// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client.Loggers;

internal sealed class SimpleConsoleLogger<TCategoryName> : ILogger<TCategoryName>
{
    internal static SimpleConsoleLogger<TCategoryName> Instance { get; } = new();

    private SimpleConsoleLogger() { }

    IDisposable? ILogger.BeginScope<TState>(TState state) => throw new NotImplementedException();

    bool ILogger.IsEnabled(LogLevel logLevel) => true;

    void ILogger.Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var logMessage = formatter?.Invoke(state, exception);
        Console.WriteLine($"""
            {logLevel}[{eventId}]: {logMessage}
            """);
    }
}
