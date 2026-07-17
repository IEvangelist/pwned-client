// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Globalization;

namespace HaveIBeenPwned.Client;

internal sealed partial class DefaultPwnedClient : IPwnedClient
{
    Task<PwnedPassword> IPwnedPasswordsClient.GetPwnedPasswordAsync(
        string plainTextPassword,
        bool addPadding,
        CancellationToken cancellationToken) =>
        GetPwnedPasswordAsync(
            plainTextPassword,
            addPadding,
            useNtlm: false,
            cancellationToken);

    Task<PwnedPassword> IPwnedPasswordsClient.GetPwnedPasswordWithNtlmAsync(
        string plainTextPassword,
        bool addPadding,
        CancellationToken cancellationToken) =>
        GetPwnedPasswordAsync(
            plainTextPassword,
            addPadding,
            useNtlm: true,
            cancellationToken);

    private async Task<PwnedPassword> GetPwnedPasswordAsync(
        string plainTextPassword,
        bool addPadding,
        bool useNtlm,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(plainTextPassword);

        var passwordHash = useNtlm
            ? plainTextPassword.ToNtlmHash()!
            : plainTextPassword.ToSha1Hash()!;
        var mode = useNtlm ? "?mode=ntlm" : "";

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"range/{passwordHash[..5]}{mode}");
        if (addPadding)
        {
            request.Headers.Add("Add-Padding", "true");
        }

        using var response = await SendAsync(
                PasswordsClient,
                request,
                notFoundIsEmpty: false,
                cancellationToken)
            .ConfigureAwait(false);
        var responseText = await response!.Content
            .ReadAsStringAsync(cancellationToken)
            .ConfigureAwait(false);

        return ParsePasswordRangeResponseText(
            new PwnedPassword { PlainTextPassword = plainTextPassword },
            responseText,
            passwordHash);
    }

    internal static PwnedPassword ParsePasswordRangeResponseText(
        PwnedPassword pwnedPassword,
        string? passwordRangeResponseText,
        string passwordHash)
    {
        var result = pwnedPassword with
        {
            HashedPassword = passwordHash,
            IsPwned = false,
            PwnedCount = 0
        };

        if (string.IsNullOrEmpty(passwordRangeResponseText))
        {
            return result;
        }

        var expectedSuffix = passwordHash[5..];
        foreach (var rawLine in passwordRangeResponseText.Split(
            '\n',
            StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var line = rawLine.AsSpan().TrimEnd('\r');
            var separatorIndex = line.IndexOf(':');
            if (separatorIndex <= 0 ||
                separatorIndex == line.Length - 1 ||
                !line[..separatorIndex].Equals(
                    expectedSuffix,
                    StringComparison.OrdinalIgnoreCase) ||
                !long.TryParse(
                    line[(separatorIndex + 1)..],
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out var count))
            {
                continue;
            }

            return result with
            {
                PwnedCount = count,
                IsPwned = count > 0
            };
        }

        return result;
    }
}
