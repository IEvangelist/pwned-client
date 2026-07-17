// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using HaveIBeenPwned.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HaveIBeenPwned.ClientTests;

[Collection("Telemetry")]
public sealed class PwnedClientTelemetryTests
{
    [Fact]
    public async Task ClientActivityContainsStablePiiSafeTags()
    {
        const string account = "user+private@example.com";
        Activity? stoppedActivity = null;
        using var listener = new ActivityListener
        {
            ShouldListenTo = static source =>
                source.Name == PwnedClientTelemetry.ActivitySourceName,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) =>
                ActivitySamplingResult.AllData,
            ActivityStopped = activity => stoppedActivity = activity
        };
        ActivitySource.AddActivityListener(listener);
        using var factory = new TestHttpClientFactory(_ =>
            TestHttpClientFactory.Json("""[{"name":"Adobe"}]"""));
        var client = CreateClient(factory);

        await client.GetBreachHeadersForAccountAsync(account);

        Assert.NotNull(stoppedActivity);
        Assert.Equal("hibp.breaches.account", stoppedActivity.OperationName);
        Assert.Equal(ActivityStatusCode.Ok, stoppedActivity.Status);
        Assert.Equal(
            "/breachedaccount/{account}",
            stoppedActivity.GetTagItem("url.template"));
        Assert.Equal(
            "haveibeenpwned.com",
            stoppedActivity.GetTagItem("server.address"));
        Assert.Equal("success", stoppedActivity.GetTagItem("hibp.outcome"));
        Assert.DoesNotContain(
            stoppedActivity.TagObjects,
            tag => tag.Value?.ToString()?.Contains(
                account,
                StringComparison.OrdinalIgnoreCase) is true);
        Assert.Null(stoppedActivity.GetTagItem("url.full"));
    }

    [CollectionDefinition("Telemetry", DisableParallelization = true)]
    public sealed class TelemetryCollection;

    [Fact]
    public async Task MetricsRecordSuccessFailureDurationAndRateLimit()
    {
        List<MetricMeasurement> measurements = [];
        using var listener = new MeterListener();
        listener.InstrumentPublished = static (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == PwnedClientTelemetry.MeterName)
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>(
            (instrument, measurement, tags, _) =>
                measurements.Add(new(
                    instrument.Name,
                    measurement,
                    ToDictionary(tags))));
        listener.SetMeasurementEventCallback<double>(
            (instrument, measurement, tags, _) =>
                measurements.Add(new(
                    instrument.Name,
                    measurement,
                    ToDictionary(tags))));
        listener.Start();

        var callCount = 0;
        using var factory = new TestHttpClientFactory(_ =>
        {
            callCount++;
            if (callCount == 1)
            {
                return TestHttpClientFactory.Json("""["Email addresses"]""");
            }

            var response = TestHttpClientFactory.Json(
                """{"statusCode":429,"message":"Rate limit exceeded."}""",
                HttpStatusCode.TooManyRequests);
            response.Headers.RetryAfter =
                new System.Net.Http.Headers.RetryConditionHeaderValue(
                    TimeSpan.FromSeconds(2));
            return response;
        });
        var client = CreateClient(factory);

        await client.GetDataClassesAsync();
        await Assert.ThrowsAsync<PwnedApiException>(
            () => client.GetDataClassesAsync());

        Assert.Equal(
            2,
            measurements.Count(
                static measurement =>
                    measurement.InstrumentName ==
                    PwnedClientTelemetry.RequestCountInstrumentName));
        Assert.Equal(
            2,
            measurements.Count(
                static measurement =>
                    measurement.InstrumentName ==
                    PwnedClientTelemetry.RequestDurationInstrumentName));
        Assert.Single(
            measurements,
            static measurement =>
                measurement.InstrumentName ==
                PwnedClientTelemetry.FailureCountInstrumentName);
        var rateLimit = Assert.Single(
            measurements,
            static measurement =>
                measurement.InstrumentName ==
                PwnedClientTelemetry.RateLimitCountInstrumentName);
        Assert.Equal("breaches.data_classes", rateLimit.Tags["hibp.operation"]);
        Assert.Equal("error", rateLimit.Tags["hibp.outcome"]);
        Assert.Equal(429, rateLimit.Tags["http.response.status_code"]);
    }

    [Fact]
    public async Task CancellationIsRecordedWithoutFailureOrPii()
    {
        List<MetricMeasurement> measurements = [];
        using var listener = new MeterListener();
        listener.InstrumentPublished = static (instrument, meterListener) =>
        {
            if (instrument.Meter.Name == PwnedClientTelemetry.MeterName)
            {
                meterListener.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>(
            (instrument, measurement, tags, _) =>
                measurements.Add(new(
                    instrument.Name,
                    measurement,
                    ToDictionary(tags))));
        listener.SetMeasurementEventCallback<double>(
            (instrument, measurement, tags, _) =>
                measurements.Add(new(
                    instrument.Name,
                    measurement,
                    ToDictionary(tags))));
        listener.Start();

        using var factory = new TestHttpClientFactory(
            async (_, cancellationToken) =>
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                return TestHttpClientFactory.Json("[]");
            });
        var client = CreateClient(factory);
        using var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.GetDataClassesAsync(cancellationTokenSource.Token));

        var request = Assert.Single(
            measurements,
            static measurement =>
                measurement.InstrumentName ==
                PwnedClientTelemetry.RequestCountInstrumentName);
        Assert.Equal("canceled", request.Tags["hibp.outcome"]);
        Assert.DoesNotContain(
            measurements,
            static measurement =>
                measurement.InstrumentName ==
                PwnedClientTelemetry.FailureCountInstrumentName);
    }

    private static IPwnedClient CreateClient(TestHttpClientFactory factory) =>
        new DefaultPwnedClient(
            factory,
            NullLogger<DefaultPwnedClient>.Instance);

    private static Dictionary<string, object?> ToDictionary(
        ReadOnlySpan<KeyValuePair<string, object?>> tags) =>
        tags.ToArray().ToDictionary(
            static tag => tag.Key,
            static tag => tag.Value,
            StringComparer.Ordinal);

    private sealed record class MetricMeasurement(
        string InstrumentName,
        double Value,
        Dictionary<string, object?> Tags);
}
