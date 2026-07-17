// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedDomainClient
{
    async Task<DomainVerificationDnsToken> IPwnedDomainClient.GenerateDomainVerificationDnsTokenAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return await PostJsonAsync(
                "domainverification/generatednstoken",
                new DomainVerificationRequest { DomainName = domain.Trim() },
                InternalSourceGeneratorContext.Default.DomainVerificationRequest,
                SourceGeneratorContext.Default.DomainVerificationDnsToken,
                cancellationToken)
            .ConfigureAwait(false)
            ?? throw new JsonException(
                "The HIBP API returned an empty DNS verification token response.");
    }

    async Task IPwnedDomainClient.VerifyDomainViaDnsAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        await PostJsonAsync(
                "domainverification/verifydnstoken",
                new DomainVerificationRequest { DomainName = domain.Trim() },
                InternalSourceGeneratorContext.Default.DomainVerificationRequest,
                cancellationToken)
            .ConfigureAwait(false);
    }

    async Task IPwnedDomainClient.SendDomainVerificationEmailAsync(
        string domain,
        DomainVerificationEmailAlias emailAlias,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        await PostJsonAsync(
                "domainverification/sendemail",
                new DomainVerificationEmailRequest
                {
                    DomainName = domain.Trim(),
                    EmailAlias = GetEmailAlias(emailAlias)
                },
                InternalSourceGeneratorContext.Default.DomainVerificationEmailRequest,
                cancellationToken)
            .ConfigureAwait(false);
    }

    async Task<DomainBreaches?> IPwnedDomainClient.GetBreachedDomainAsync(
        string domain,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(domain);

        return await GetJsonAsync(
                HibpClient,
                $"breacheddomain/{EscapePathSegment(domain)}",
                SourceGeneratorContext.Default.DomainBreaches,
                notFoundIsEmpty: true,
                cancellationToken)
            .ConfigureAwait(false);
    }

    async Task<SubscribedDomain[]> IPwnedDomainClient.GetSubscribedDomainsAsync(
        CancellationToken cancellationToken) =>
        await GetJsonAsync(
                HibpClient,
                "subscribeddomains",
                SourceGeneratorContext.Default.SubscribedDomainArray,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false) ?? [];

    IAsyncEnumerable<SubscribedDomain?> IPwnedDomainClient.GetSubscribedDomainsAsAsyncEnumerable(
        CancellationToken cancellationToken) =>
        GetJsonArrayAsync(
            HibpClient,
            "subscribeddomains",
            SourceGeneratorContext.Default.SubscribedDomain,
            notFoundIsEmpty: false,
            cancellationToken);

    private static string GetEmailAlias(DomainVerificationEmailAlias emailAlias) =>
        emailAlias switch
        {
            DomainVerificationEmailAlias.Admin => "admin",
            DomainVerificationEmailAlias.Hostmaster => "hostmaster",
            DomainVerificationEmailAlias.Info => "info",
            DomainVerificationEmailAlias.Security => "security",
            DomainVerificationEmailAlias.Webmaster => "webmaster",
            _ => throw new ArgumentOutOfRangeException(
                nameof(emailAlias),
                emailAlias,
                "The email alias is not supported by the HIBP API.")
        };
}
