// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace HaveIBeenPwned.Client;

/// <summary>
/// Identifies the OpenTelemetry-compatible diagnostics emitted by the HIBP client.
/// </summary>
public static class PwnedClientTelemetry
{
    /// <summary>
    /// The name of the <see cref="ActivitySource"/> used for client traces.
    /// </summary>
    public const string ActivitySourceName = "HaveIBeenPwned.Client";

    /// <summary>
    /// The name of the <see cref="Meter"/> used for client metrics.
    /// </summary>
    public const string MeterName = "HaveIBeenPwned.Client";

    /// <summary>
    /// The request-count instrument name.
    /// </summary>
    public const string RequestCountInstrumentName = "hibp.client.request.count";

    /// <summary>
    /// The request-duration instrument name.
    /// </summary>
    public const string RequestDurationInstrumentName = "hibp.client.request.duration";

    /// <summary>
    /// The failed-request instrument name.
    /// </summary>
    public const string FailureCountInstrumentName = "hibp.client.request.failure.count";

    /// <summary>
    /// The rate-limit instrument name.
    /// </summary>
    public const string RateLimitCountInstrumentName = "hibp.client.rate_limit.count";

    private static readonly string? s_version =
        typeof(PwnedClientTelemetry).Assembly.GetName().Version?.ToString();

    internal static readonly ActivitySource ActivitySource =
        new(ActivitySourceName, s_version);

    internal static readonly Meter Meter =
        new(MeterName, s_version);

    internal static readonly Counter<long> RequestCount = Meter.CreateCounter<long>(
        RequestCountInstrumentName,
        unit: "{request}",
        description: "The number of logical HIBP client requests.");

    internal static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
        RequestDurationInstrumentName,
        unit: "s",
        description: "The duration of logical HIBP client requests.");

    internal static readonly Counter<long> FailureCount = Meter.CreateCounter<long>(
        FailureCountInstrumentName,
        unit: "{request}",
        description: "The number of failed HIBP client requests.");

    internal static readonly Counter<long> RateLimitCount = Meter.CreateCounter<long>(
        RateLimitCountInstrumentName,
        unit: "{request}",
        description: "The number of HIBP API rate-limit responses.");
}
