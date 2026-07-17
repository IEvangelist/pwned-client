// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

/// <summary>
/// Represents a non-success response returned by the Have I Been Pwned API.
/// </summary>
public sealed class PwnedApiException : HttpRequestException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PwnedApiException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="responseContent">The response body returned by the API, if any.</param>
    /// <param name="retryAfter">The delay requested by the API before retrying, if any.</param>
    /// <param name="innerException">The exception that caused this exception, if any.</param>
    public PwnedApiException(
        string message,
        HttpStatusCode statusCode,
        string? responseContent = null,
        TimeSpan? retryAfter = null,
        Exception? innerException = null)
        : base(message, innerException, statusCode)
    {
        ResponseContent = responseContent;
        RetryAfter = retryAfter;
    }

    /// <summary>
    /// Gets the response body returned by the API, if any.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Gets the delay requested by the API before retrying, if any.
    /// </summary>
    public TimeSpan? RetryAfter { get; }
}
