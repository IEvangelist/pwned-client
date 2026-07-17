# ![';-- have i been pwned? - .NET HTTP client.](https://raw.githubusercontent.com/IEvangelist/pwned-client/main/assets/pwned-header.png)

[![build](https://github.com/IEvangelist/pwned-client/actions/workflows/build-validation.yml/badge.svg)](https://github.com/IEvangelist/pwned-client/actions/workflows/build-validation.yml)
[![code analysis](https://github.com/IEvangelist/pwned-client/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/IEvangelist/pwned-client/actions/workflows/codeql-analysis.yml)
[![NuGet](https://img.shields.io/nuget/v/HaveIBeenPwned.Client.svg?style=flat)](https://www.nuget.org/packages/HaveIBeenPwned.Client)
![NuGet downloads](https://img.shields.io/nuget/dt/HaveIBeenPwned.Client?color=blue&label=nuget%20downloads&logo=nuget)

`HaveIBeenPwned.Client` is an unofficial, AOT-compatible .NET client for the complete [Have I Been Pwned REST API v3](https://haveibeenpwned.com/API/v3).

The repository publishes three packages:

- [`HaveIBeenPwned.Client`](https://www.nuget.org/packages/HaveIBeenPwned.Client)
- [`HaveIBeenPwned.Client.Abstractions`](https://www.nuget.org/packages/HaveIBeenPwned.Client.Abstractions)
- [`HaveIBeenPwned.Client.PollyExtensions`](https://www.nuget.org/packages/HaveIBeenPwned.Client.PollyExtensions)

## Supported API surface

| Area | Operations | Client | Access |
| --- | --- | --- | --- |
| Email search | Direct account search, filtered/truncated responses, six-character k-anonymity search | `IPwnedBreachesClient` | Core, Pro, or High RPM; k-anonymity requires Pro or High RPM |
| Domain verification | Generate DNS token, verify DNS token, send verification email | `IPwnedDomainClient` | Pro |
| Domain search | Breached domain and subscribed domains | `IPwnedDomainClient` | Core or Pro |
| Breaches | All breaches, single breach, latest breach, data classes | `IPwnedBreachesClient` | Public |
| Stealer logs | By email, website domain, or email domain | `IPwnedStealerLogsClient` | Pro |
| Pastes | Pastes for an email address | `IPwnedPastesClient` | Core, Pro, or High RPM |
| Subscription | Current status and capabilities | `IPwnedClient` | Authenticated |
| Pwned Passwords | SHA-1 and NTLM range searches with optional padding | `IPwnedPasswordsClient` | Public |

`IPwnedClient` aggregates all specialized clients.

## Getting started

Install the client:

```shell
dotnet add package HaveIBeenPwned.Client
```

### Dependency injection

```csharp
builder.Services.AddPwnedServices(options =>
{
    options.ApiKey = builder.Configuration["HibpOptions:ApiKey"];
    options.UserAgent = "my-service/1.0";
});
```

The API key is optional when only public breach-catalogue or Pwned Passwords operations are used:

```csharp
builder.Services.AddPwnedServices(options =>
{
    options.UserAgent = "my-public-service/1.0";
});
```

Resolve either a specialized client or `IPwnedClient`:

```csharp
var breaches = serviceProvider.GetRequiredService<IPwnedBreachesClient>();
var allApis = serviceProvider.GetRequiredService<IPwnedClient>();
```

### Without dependency injection

```csharp
IPwnedClient publicClient = new PwnedClient();
IPwnedClient authenticatedClient = new PwnedClient("<HIBP API key>");
```

## Examples

### Search an account directly

```csharp
var breaches = await client.GetBreachesForAccountAsync(
    "user@example.com",
    includeUnverified: false,
    domain: "example.com",
    cancellationToken);
```

### Search an account with k-anonymity

```csharp
var breachHeaders =
    await client.GetBreachHeadersForAccountUsingKAnonymityAsync(
        "user@example.com",
        cancellationToken);
```

The client normalizes and hashes the address locally, sends only the first six SHA-1 characters, matches the returned suffix locally, and does not expose nonmatching range entries.

### Verify a domain

```csharp
var token = await client.GenerateDomainVerificationDnsTokenAsync(
    "example.com",
    cancellationToken);

// Publish token.TxtRecordValue as a DNS TXT record, then:
await client.VerifyDomainViaDnsAsync("example.com", cancellationToken);
```

Email verification is also supported:

```csharp
await client.SendDomainVerificationEmailAsync(
    "example.com",
    DomainVerificationEmailAlias.Security,
    cancellationToken);
```

### Search Pwned Passwords

```csharp
var result = await client.GetPwnedPasswordAsync(
    plainTextPassword,
    addPadding: true,
    cancellationToken);

var ntlmResult = await client.GetPwnedPasswordWithNtlmAsync(
    plainTextPassword,
    addPadding: true,
    cancellationToken);
```

Only a five-character hash prefix is sent to the Pwned Passwords service. Padding rows with a zero count are discarded.

## Error behavior

Documented `404 Not Found` responses map to the operation's no-result shape, such as an empty collection or `null`. Other HTTP failures throw `PwnedApiException`:

```csharp
try
{
    await client.GetBreachesForAccountAsync("user@example.com");
}
catch (PwnedApiException exception)
{
    Console.WriteLine(exception.StatusCode);
    Console.WriteLine(exception.ResponseContent);
    Console.WriteLine(exception.RetryAfter);
}
```

Cancellation, transport failures, and JSON failures are never converted into empty successful-looking results.

## OpenTelemetry and Aspire

The client emits dependency-free .NET diagnostics through a stable `ActivitySource` and `Meter`. Register them with an existing OpenTelemetry setup:

```csharp
builder.Services
    .AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing.AddSource(PwnedClientTelemetry.ActivitySourceName))
    .WithMetrics(metrics =>
        metrics.AddMeter(PwnedClientTelemetry.MeterName));
```

This integrates with standard OpenTelemetry exporters and Aspire service defaults. If an Aspire `ServiceDefaults` project already configures OpenTelemetry, add the source and meter to that existing configuration.

The following instruments are emitted:

| Instrument | Type | Description |
| --- | --- | --- |
| `hibp.client.request.count` | Counter | Logical client requests |
| `hibp.client.request.duration` | Histogram, seconds | Logical request duration |
| `hibp.client.request.failure.count` | Counter | Failed requests |
| `hibp.client.rate_limit.count` | Counter | HTTP 429 responses |

Spans and metrics use low-cardinality operation, API surface, method, outcome, and status tags. The client does not attach email addresses, domains, passwords, hashes, API keys, response bodies, or raw URLs to its own telemetry.

Authenticated HIBP endpoints place email addresses or domains in the HTTP path. If standard `HttpClient` instrumentation is also enabled, configure its filtering or redaction so `url.full` is not exported for these hosts. The library-level spans remain available even when the lower-level HTTP spans are filtered.

## Resilience

Install `HaveIBeenPwned.Client.PollyExtensions` to configure the standard .NET HTTP resilience pipeline. Its default policy:

- honors the HIBP `Retry-After` header;
- retries HTTP 429 and 503 responses;
- applies jitter, a circuit breaker, and an overall timeout;
- redacts the `hibp-api-key` header from HTTP logs.

Custom `HttpStandardResilienceOptions` can be supplied through the package's `AddPwnedServices` overloads.

## Configuration

| Key | Type | Default |
| --- | --- | --- |
| `HibpOptions__ApiKey` | `string?` | `null` |
| `HibpOptions__UserAgent` | `string` | `.NET HIBP Client/{AssemblyFileVersion}` |
| `HibpOptions__SubscriptionLevel` | `HibpSubscriptionLevel?` | `null` |

Every API request includes a user agent. Authenticated email, domain, paste, stealer-log, and subscription operations require a valid [HIBP API key](https://haveibeenpwned.com/API/Key).

## Privacy, security, and attribution

- Do not run password searches for every keystroke; wait until password entry is complete.
- Do not log or export plaintext passwords or `PwnedPassword.PlainTextPassword`.
- K-anonymity range results that do not match the requested account or password must not be retained.
- Breach and paste API data is licensed under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/); consuming experiences must attribute Have I Been Pwned.
- Pwned Passwords does not require attribution.
