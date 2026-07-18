export interface ApiParam {
  name: string;
  type: string;
  required?: boolean;
  desc: string;
}

export interface ApiReturn {
  type: string;
  desc: string;
}

export interface ApiException {
  type: string;
  when: string;
}

export interface ApiStatus {
  code: string;
  meaning: string;
}

export interface ApiTestInput {
  name: string;
  label: string;
  placeholder?: string;
  required?: boolean;
  hashType?: "sha1" | "ntlm";
  options?: { label: string; value: string }[];
}

export interface ApiEndpoint {
  method: "GET" | "POST";
  url: string;
  docsUrl: string;
  testInputs?: ApiTestInput[];
}

export interface ApiOperation {
  id: string;
  name: string;
  signature: string;
  summary: string;
  auth: string;
  endpoint: ApiEndpoint;
  params: ApiParam[];
  returns: ApiReturn;
  streaming?: string;
  example: string;
  output: string;
  outputLang?: "text" | "json" | "csharp";
  outputNote?: string;
  exceptions: ApiException[];
  statusCodes: ApiStatus[];
  notes?: string[];
}

export interface ApiClient {
  name: string;
  desc: string;
}

export interface ApiInstrument {
  instrument: string;
  kind: string;
  unit: string;
  desc: string;
}

export interface ApiConstant {
  name: string;
  value: string;
  desc: string;
}

export interface ApiSurface {
  title: string;
  summary: string;
  icon: string;
  clients: ApiClient[];
  inject: string;
  registration?: string;
  operations: ApiOperation[];
  constants?: ApiConstant[];
  instruments?: ApiInstrument[];
  closingExample?: { label: string; code: string; output?: string; outputLang?: "text" | "json" | "csharp"; outputNote?: string };
}

const commonRateLimit: ApiStatus = {
  code: "429",
  meaning: "Rate limit exceeded. The Retry-After header is surfaced on PwnedApiException.RetryAfter.",
};
const serviceUnavailable: ApiStatus = {
  code: "503",
  meaning: "Service temporarily unavailable. Retried by the resilience pipeline when configured.",
};

export const apiDocs: Record<string, ApiSurface> = {
  breaches: {
    title: "Breaches",
    summary:
      "Browse the public breach catalogue or look up the breaches tied to a specific account. Account searches are authenticated; the catalogue endpoints are open.",
    icon: "ph-door-open",
    clients: [
      {
        name: "IPwnedBreachesClient",
        desc: "Breach catalogue lookups plus account exposure searches, including the privacy-preserving k-anonymity variant.",
      },
    ],
    inject:
      "Inject IPwnedBreachesClient for breach work, or the aggregate IPwnedClient when you need every surface. Both resolve to the same singleton.",
    registration: `using HaveIBeenPwned.Client;

var builder = WebApplication.CreateBuilder(args);

// Binds the "HibpOptions" section from appsettings.json.
builder.Services.AddPwnedServices(
    builder.Configuration.GetSection(nameof(HibpOptions)));

var app = builder.Build();`,
    operations: [
      {
        id: "get-breaches-for-account-async",
        name: "GetBreachesForAccountAsync",
        signature:
          "Task<BreachDetails[]> GetBreachesForAccountAsync(string account, bool includeUnverified = true, string? domain = null, CancellationToken ct = default)",
        summary:
          "Returns the full breach details for every breach an account has appeared in. This is the untruncated search, so each result carries the complete BreachDetails payload.",
        auth: "API key",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/breachedaccount/{account}?truncateResponse=false&includeUnverified={includeUnverified}&domain={domain}",
          docsUrl: "https://haveibeenpwned.com/API/v3#BreachesForAccount",
        },
        params: [
          { name: "account", type: "string", required: true, desc: "Email address or username to search. Trimmed and URL encoded for you." },
          { name: "includeUnverified", type: "bool", desc: "Include unverified breaches. Defaults to true." },
          { name: "domain", type: "string?", desc: "Restrict results to a single breach domain." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<BreachDetails[]>", desc: "Full breach records, or an empty array when the account is clean (HTTP 404)." },
        streaming: "GetBreachesForAccountAsAsyncEnumerable",
        example: `public sealed class ExposureReport(IPwnedBreachesClient breaches)
{
    public async Task PrintAsync(string account)
    {
        BreachDetails[] results =
            await breaches.GetBreachesForAccountAsync(account);

        foreach (var breach in results)
        {
            Console.WriteLine(
                $"{breach.Title} ({breach.BreachDate:yyyy-MM-dd}) " +
                $"exposed {breach.PwnCount:N0} accounts.");
        }
    }
}`,
        output: `Adobe (2013-10-04) exposed 152,445,165 accounts.
LinkedIn (2012-05-05) exposed 164,611,595 accounts.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "account is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404 (which maps to an empty array)." },
        ],
        statusCodes: [
          { code: "200", meaning: "Breaches found for the account." },
          { code: "401", meaning: "Missing or invalid API key." },
          { code: "404", meaning: "No breaches for the account. Returned to you as an empty array." },
          commonRateLimit,
          serviceUnavailable,
        ],
        notes: [
          "Account searches require an API key set on HibpOptions.ApiKey.",
          "Prefer the k-anonymity variant when you do not need full breach detail and want to avoid sending the full address.",
        ],
      },
      {
        id: "get-breach-headers-for-account-async",
        name: "GetBreachHeadersForAccountAsync",
        signature:
          "Task<BreachHeader[]> GetBreachHeadersForAccountAsync(string account, bool includeUnverified, string? domain = null, CancellationToken ct = default)",
        summary:
          "The truncated account search. Returns only breach names, which is the lightest way to answer \"has this account been in a breach?\".",
        auth: "API key",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/breachedaccount/{account}?truncateResponse=true&includeUnverified={includeUnverified}&domain={domain}",
          docsUrl: "https://haveibeenpwned.com/API/v3#BreachesForAccount",
        },
        params: [
          { name: "account", type: "string", required: true, desc: "Email address or username to search." },
          { name: "includeUnverified", type: "bool", desc: "Include unverified breaches." },
          { name: "domain", type: "string?", desc: "Restrict results to a single breach domain." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<BreachHeader[]>", desc: "Breach headers (Name only), or an empty array when the account is clean." },
        streaming: "GetBreachHeadersForAccountAsAsyncEnumerable",
        example: `BreachHeader[] headers =
    await breaches.GetBreachHeadersForAccountAsync(
        "account@example.com",
        includeUnverified: false);

Console.WriteLine($"{headers.Length} breaches on record.");`,
        output: `3 breaches on record.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "account is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404." },
        ],
        statusCodes: [
          { code: "200", meaning: "Breach headers found." },
          { code: "401", meaning: "Missing or invalid API key." },
          { code: "404", meaning: "No breaches. Returned as an empty array." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-breach-headers-for-account-using-k-anonymity-async",
        name: "GetBreachHeadersForAccountUsingKAnonymityAsync",
        signature:
          "Task<BreachHeader[]> GetBreachHeadersForAccountUsingKAnonymityAsync(string account, CancellationToken ct = default)",
        summary:
          "Searches breaches by sending only the first six characters of the SHA-1 hash of the normalized address. The client filters the returned range locally so the full address never leaves your process.",
        auth: "API key",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/breachedaccount/range/{hashPrefix}",
          docsUrl: "https://haveibeenpwned.com/API/v3#BreachedAccountRange",
        },
        params: [
          { name: "account", type: "string", required: true, desc: "Email address to search. Only a six-character hash prefix is sent." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<BreachHeader[]>", desc: "Locally matched breach headers, or an empty array." },
        streaming: "GetBreachHeadersForAccountUsingKAnonymityAsAsyncEnumerable",
        example: `BreachHeader[] headers =
    await breaches.GetBreachHeadersForAccountUsingKAnonymityAsync(
        "account@example.com");

foreach (var header in headers)
{
    Console.WriteLine(header.Name);
}`,
        output: `Adobe
LinkedIn
Dropbox`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "account is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status while fetching the hash range." },
        ],
        statusCodes: [
          { code: "200", meaning: "Hash range returned and filtered locally." },
          { code: "401", meaning: "Missing or invalid API key." },
          commonRateLimit,
          serviceUnavailable,
        ],
        notes: ["Matching happens on your machine, so range entries that do not match the requested address must never be retained."],
      },
      {
        id: "get-breaches-async",
        name: "GetBreachesAsync",
        signature:
          "Task<BreachHeader[]> GetBreachesAsync(string? domain = default, bool? isSpamList = null, CancellationToken ct = default)",
        summary: "Lists breach headers from the public catalogue, with optional domain and spam-list filters. No API key required.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/breaches?domain={domain}&isSpamList={isSpamList}",
          docsUrl: "https://haveibeenpwned.com/API/v3#AllBreaches",
          testInputs: [
            { name: "domain", label: "Domain", placeholder: "adobe.com" },
            {
              name: "isSpamList",
              label: "Spam list",
              options: [
                { label: "Any", value: "" },
                { label: "Only spam lists", value: "true" },
                { label: "Exclude spam lists", value: "false" },
              ],
            },
          ],
        },
        params: [
          { name: "domain", type: "string?", desc: "Filter to breaches for a single domain." },
          { name: "isSpamList", type: "bool?", desc: "Filter to breaches that are, or are not, spam lists." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<BreachHeader[]>", desc: "Breach headers for the whole catalogue, or an empty array." },
        streaming: "GetBreachesAsAsyncEnumerable",
        example: `BreachHeader[] all = await breaches.GetBreachesAsync();

Console.WriteLine($"{all.Length} breaches in the catalogue.");`,
        output: `857 breaches in the catalogue.`,
        outputLang: "text",
        exceptions: [{ type: "PwnedApiException", when: "Any non-success status." }],
        statusCodes: [
          { code: "200", meaning: "Catalogue returned." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-all-breach-details-async",
        name: "GetAllBreachDetailsAsync",
        signature:
          "Task<BreachDetails[]> GetAllBreachDetailsAsync(string? domain = default, bool? isSpamList = null, CancellationToken ct = default)",
        summary: "Like GetBreachesAsync, but returns the full BreachDetails for every breach. Useful for building a local mirror of the catalogue.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/breaches?domain={domain}&isSpamList={isSpamList}",
          docsUrl: "https://haveibeenpwned.com/API/v3#AllBreaches",
          testInputs: [
            { name: "domain", label: "Domain", placeholder: "adobe.com" },
            {
              name: "isSpamList",
              label: "Spam list",
              options: [
                { label: "Any", value: "" },
                { label: "Only spam lists", value: "true" },
                { label: "Exclude spam lists", value: "false" },
              ],
            },
          ],
        },
        params: [
          { name: "domain", type: "string?", desc: "Filter to breaches for a single domain." },
          { name: "isSpamList", type: "bool?", desc: "Filter on the spam-list flag." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<BreachDetails[]>", desc: "Full breach details for the catalogue, or an empty array." },
        streaming: "GetAllBreachDetailsAsAsyncEnumerable",
        example: `BreachDetails[] details =
    await breaches.GetAllBreachDetailsAsync(isSpamList: false);

var largest = details.MaxBy(static b => b.PwnCount);
Console.WriteLine($"Largest: {largest?.Title} ({largest?.PwnCount:N0}).");`,
        output: `Largest: Collection #1 (772,904,991).`,
        outputLang: "text",
        exceptions: [{ type: "PwnedApiException", when: "Any non-success status." }],
        statusCodes: [
          { code: "200", meaning: "Catalogue returned." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-breach-async",
        name: "GetBreachAsync",
        signature: "Task<BreachDetails?> GetBreachAsync(string breachName, CancellationToken ct = default)",
        summary: "Fetches a single breach by its stable Name. Returns null when no breach matches.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/breach/{breachName}",
          docsUrl: "https://haveibeenpwned.com/API/v3#SingleBreach",
          testInputs: [
            { name: "breachName", label: "Breach name", placeholder: "Adobe", required: true },
          ],
        },
        params: [
          { name: "breachName", type: "string", required: true, desc: "The stable breach name, for example \"Adobe\"." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<BreachDetails?>", desc: "The breach record, or null when the name is unknown (HTTP 404)." },
        example: `BreachDetails? adobe = await breaches.GetBreachAsync("Adobe");

if (adobe is not null)
{
    Console.WriteLine(string.Join(", ", adobe.DataClasses));
}`,
        output: `Email addresses, Password hints, Passwords, Usernames`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "breachName is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404." },
        ],
        statusCodes: [
          { code: "200", meaning: "Breach found." },
          { code: "404", meaning: "Unknown breach name. Returned to you as null." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-data-classes-async",
        name: "GetDataClassesAsync",
        signature: "Task<string[]> GetDataClassesAsync(CancellationToken ct = default)",
        summary: "Lists every data class the system tracks, for example \"Email addresses\" or \"Phone numbers\". Handy for building filter UIs.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/dataclasses",
          docsUrl: "https://haveibeenpwned.com/API/v3#AllDataClasses",
          testInputs: [],
        },
        params: [{ name: "ct", type: "CancellationToken", desc: "Signals cancellation." }],
        returns: { type: "Task<string[]>", desc: "All known data classes, or an empty array." },
        streaming: "GetDataClassesAsAsyncEnumerable",
        example: `string[] classes = await breaches.GetDataClassesAsync();

Console.WriteLine($"{classes.Length} data classes tracked.");`,
        output: `140 data classes tracked.`,
        outputLang: "text",
        exceptions: [{ type: "PwnedApiException", when: "Any non-success status." }],
        statusCodes: [
          { code: "200", meaning: "Data classes returned." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-latest-breach-async",
        name: "GetLatestBreachAsync",
        signature: "Task<BreachDetails?> GetLatestBreachAsync(CancellationToken ct = default)",
        summary: "Returns the most recently added breach, ordered by AddedDate. Returns null when the catalogue is empty.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/latestbreach",
          docsUrl: "https://haveibeenpwned.com/API/v3#LatestBreach",
          testInputs: [],
        },
        params: [{ name: "ct", type: "CancellationToken", desc: "Signals cancellation." }],
        returns: { type: "Task<BreachDetails?>", desc: "The newest breach, or null." },
        example: `BreachDetails? latest = await breaches.GetLatestBreachAsync();

Console.WriteLine($"Newest: {latest?.Title} added {latest?.AddedDate:u}.");`,
        output: `Newest: Example Forum added 2024-11-03 09:14:00Z.`,
        outputLang: "text",
        exceptions: [{ type: "PwnedApiException", when: "Any non-success status other than 404." }],
        statusCodes: [
          { code: "200", meaning: "Latest breach returned." },
          { code: "404", meaning: "No breach available. Returned as null." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
    ],
  },

  passwords: {
    title: "Pwned passwords",
    summary:
      "Check whether a password has appeared in a breach without ever sending it. The client hashes locally and only transmits a five-character prefix using k-anonymity. No API key required.",
    icon: "ph-key",
    clients: [
      {
        name: "IPwnedPasswordsClient",
        desc: "SHA-1 and NTLM range queries that keep the plaintext password on your machine.",
      },
    ],
    inject:
      "Inject IPwnedPasswordsClient. Password checks need no API key, so this surface works with an empty HibpOptions as long as a User-Agent is set.",
    registration: `using HaveIBeenPwned.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPwnedServices(options =>
{
    options.UserAgent = "MyApp/1.0";
});

var app = builder.Build();`,
    operations: [
      {
        id: "get-pwned-password-async",
        name: "GetPwnedPasswordAsync",
        signature: "Task<PwnedPassword> GetPwnedPasswordAsync(string plainTextPassword, bool addPadding = false, CancellationToken ct = default)",
        summary:
          "Hashes the password with SHA-1, sends the first five hash characters, and evaluates the returned range locally. IsPwned is false when no suffix matches.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://api.pwnedpasswords.com/range/{hashPrefix}",
          docsUrl: "https://haveibeenpwned.com/API/v3#PwnedPasswords",
          testInputs: [
            { name: "hashPrefix", label: "Password", placeholder: "Enter a password to check", required: true, hashType: "sha1" },
          ],
        },
        params: [
          { name: "plainTextPassword", type: "string", required: true, desc: "The candidate password. Hashed locally; never transmitted." },
          { name: "addPadding", type: "bool", desc: "Pad the response to 800-1000 records for extra privacy on the wire. Defaults to false." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<PwnedPassword>", desc: "Always a value. IsPwned and PwnedCount describe the result; PwnedCount is 0 on no match." },
        example: `public sealed class SignupValidator(IPwnedPasswordsClient passwords)
{
    public async Task<bool> IsAcceptableAsync(string candidate)
    {
        PwnedPassword result =
            await passwords.GetPwnedPasswordAsync(candidate, addPadding: true);

        return result.IsPwned is not true;
    }
}`,
        output: `{
  "PlainTextPassword": "P@ssw0rd!",
  "IsPwned": true,
  "PwnedCount": 73586,
  "HashedPassword": "21BD1...A0F9E"
}`,
        outputLang: "json",
        exceptions: [
          { type: "ArgumentNullException", when: "plainTextPassword is null or empty." },
          { type: "PwnedApiException", when: "Any non-success status while fetching the hash range." },
        ],
        statusCodes: [
          { code: "200", meaning: "Range returned. A non-matching suffix simply yields IsPwned = false." },
          commonRateLimit,
          serviceUnavailable,
        ],
        notes: [
          "Never log or persist the plaintext password or the full hash.",
          "Enable addPadding to obscure the size of the returned range from any on-path observer.",
        ],
      },
      {
        id: "get-pwned-password-with-ntlm-async",
        name: "GetPwnedPasswordWithNtlmAsync",
        signature: "Task<PwnedPassword> GetPwnedPasswordWithNtlmAsync(string plainTextPassword, bool addPadding = false, CancellationToken ct = default)",
        summary: "Identical flow to GetPwnedPasswordAsync but hashes with NTLM. Use it when validating against Active Directory style hashes.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://api.pwnedpasswords.com/range/{hashPrefix}?mode=ntlm",
          docsUrl: "https://haveibeenpwned.com/API/v3#PwnedPasswords",
          testInputs: [
            { name: "hashPrefix", label: "Password", placeholder: "Enter a password to check", required: true, hashType: "ntlm" },
          ],
        },
        params: [
          { name: "plainTextPassword", type: "string", required: true, desc: "The candidate password. Hashed with NTLM locally." },
          { name: "addPadding", type: "bool", desc: "Pad the response for extra privacy. Defaults to false." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<PwnedPassword>", desc: "The evaluation result using NTLM hashing." },
        example: `PwnedPassword result =
    await passwords.GetPwnedPasswordWithNtlmAsync("Winter2024");

Console.WriteLine(result.IsPwned is true
    ? $"Seen {result.PwnedCount:N0} times."
    : "Not found.");`,
        output: `Seen 4,112 times.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "plainTextPassword is null or empty." },
          { type: "PwnedApiException", when: "Any non-success status while fetching the hash range." },
        ],
        statusCodes: [
          { code: "200", meaning: "Range returned and evaluated locally." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "is-password-pwned-async",
        name: "IsPasswordPwnedAsync (extension)",
        signature: "Task<(bool? IsPwned, long? Count)> IsPasswordPwnedAsync(this IPwnedPasswordsClient client, string password, CancellationToken ct = default)",
        summary:
          "A convenience extension on IPwnedPasswordsClient that returns a simple tuple when you only care about the yes/no answer and the count.",
        auth: "None",
        endpoint: {
          method: "GET",
          url: "https://api.pwnedpasswords.com/range/{hashPrefix}",
          docsUrl: "https://haveibeenpwned.com/API/v3#PwnedPasswords",
          testInputs: [
            { name: "hashPrefix", label: "Password", placeholder: "Enter a password to check", required: true, hashType: "sha1" },
          ],
        },
        params: [
          { name: "password", type: "string", required: true, desc: "The candidate password." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<(bool? IsPwned, long? Count)>", desc: "A tuple of the pwned flag and the occurrence count." },
        example: `var (isPwned, count) = await passwords.IsPasswordPwnedAsync("hunter2");

Console.WriteLine($"Pwned: {isPwned}, count: {count}.");`,
        output: `Pwned: True, count: 25891.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "password is null or empty." },
          { type: "PwnedApiException", when: "Any non-success status while fetching the hash range." },
        ],
        statusCodes: [
          { code: "200", meaning: "Range returned and evaluated locally." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
    ],
  },

  pastes: {
    title: "Pastes",
    summary:
      "Find public pastes that reference an email address. Pastes are text dumps posted to sites like Pastebin. This is an authenticated surface.",
    icon: "ph-clipboard-text",
    clients: [
      { name: "IPwnedPastesClient", desc: "Retrieves paste records for a given account." },
    ],
    inject: "Inject IPwnedPastesClient. Requires an API key on HibpOptions.ApiKey.",
    registration: `using HaveIBeenPwned.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPwnedServices(options =>
{
    options.ApiKey = builder.Configuration["Hibp:ApiKey"];
    options.UserAgent = "MyApp/1.0";
});

var app = builder.Build();`,
    operations: [
      {
        id: "get-pastes-async",
        name: "GetPastesAsync",
        signature: "Task<Pastes[]> GetPastesAsync(string account, CancellationToken ct = default)",
        summary: "Returns every paste that includes the given email address, most recent first. Returns an empty array when none are found.",
        auth: "API key",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/pasteaccount/{account}",
          docsUrl: "https://haveibeenpwned.com/API/v3#PastesForAccount",
        },
        params: [
          { name: "account", type: "string", required: true, desc: "Email address to search. Trimmed and URL encoded for you." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<Pastes[]>", desc: "Paste records, or an empty array when the account appears in none (HTTP 404)." },
        streaming: "GetPastesAsAsyncEnumerable",
        example: `Pastes[] pastes = await pastesClient.GetPastesAsync("account@example.com");

foreach (var paste in pastes)
{
    Console.WriteLine($"{paste.Source}/{paste.Id}: {paste.EmailCount} emails.");
}`,
        output: `Pastebin/8Q0BvKD8: 139 emails.
AdHocUrl/example: 42 emails.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "account is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404." },
        ],
        statusCodes: [
          { code: "200", meaning: "Pastes found." },
          { code: "401", meaning: "Missing or invalid API key." },
          { code: "404", meaning: "No pastes for the account. Returned as an empty array." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
    ],
  },

  domains: {
    title: "Domains and stealer logs",
    summary:
      "Verify control of a domain, review the domains on your dashboard, and search stealer-log exposure. Domain search returns sensitive breaches, so every operation here is authenticated.",
    icon: "ph-globe-hemisphere-west",
    clients: [
      { name: "IPwnedDomainClient", desc: "Domain verification and domain-wide breach search." },
      { name: "IPwnedStealerLogsClient", desc: "Stealer-log search by email, website domain, or email domain. Requires a Pro subscription or higher." },
    ],
    inject:
      "Inject IPwnedDomainClient and IPwnedStealerLogsClient. Both need an API key; stealer logs additionally require a Pro subscription and a verified domain on your dashboard.",
    registration: `using HaveIBeenPwned.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPwnedServices(options =>
{
    options.ApiKey = builder.Configuration["Hibp:ApiKey"];
    options.SubscriptionLevel = HibpSubscriptionLevel.Pro2;
    options.UserAgent = "MyApp/1.0";
});

var app = builder.Build();`,
    operations: [
      {
        id: "generate-domain-verification-dns-token-async",
        name: "GenerateDomainVerificationDnsTokenAsync",
        signature: "Task<DomainVerificationDnsToken> GenerateDomainVerificationDnsTokenAsync(string domain, CancellationToken ct = default)",
        summary: "Requests the DNS TXT value you publish to prove control of a domain before it can be searched.",
        auth: "API key",
        endpoint: {
          method: "POST",
          url: "https://haveibeenpwned.com/api/v3/domainverification/generatednstoken",
          docsUrl: "https://haveibeenpwned.com/API/v3",
        },
        params: [
          { name: "domain", type: "string", required: true, desc: "The apex domain to verify." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<DomainVerificationDnsToken>", desc: "Carries TxtRecordValue, the string to publish as a DNS TXT record." },
        example: `DomainVerificationDnsToken token =
    await domains.GenerateDomainVerificationDnsTokenAsync("contoso.com");

Console.WriteLine($"Publish TXT: {token.TxtRecordValue}");`,
        output: `Publish TXT: hibp-verify=6f1c9a2b8e4d47f0a1b2c3d4e5f60718`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "domain is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status." },
        ],
        statusCodes: [
          { code: "200", meaning: "Token generated." },
          { code: "401", meaning: "Missing or invalid API key." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "verify-domain-via-dns-async",
        name: "VerifyDomainViaDnsAsync",
        signature: "Task VerifyDomainViaDnsAsync(string domain, CancellationToken ct = default)",
        summary: "Asks HIBP to check for the published TXT record and complete verification. Completes without a value on success.",
        auth: "API key",
        endpoint: {
          method: "POST",
          url: "https://haveibeenpwned.com/api/v3/domainverification/verifydnstoken",
          docsUrl: "https://haveibeenpwned.com/API/v3",
        },
        params: [
          { name: "domain", type: "string", required: true, desc: "The domain whose TXT record is ready to be checked." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task", desc: "Completes on success. Throws when the record is missing or invalid." },
        example: `await domains.VerifyDomainViaDnsAsync("contoso.com");
Console.WriteLine("Domain verified.");`,
        output: `Domain verified.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "domain is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Verification failed, for example the TXT record was not found (400)." },
        ],
        statusCodes: [
          { code: "200", meaning: "Verification succeeded." },
          { code: "400", meaning: "The TXT record was missing or did not match." },
          { code: "401", meaning: "Missing or invalid API key." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "send-domain-verification-email-async",
        name: "SendDomainVerificationEmailAsync",
        signature: "Task SendDomainVerificationEmailAsync(string domain, DomainVerificationEmailAlias emailAlias, CancellationToken ct = default)",
        summary: "Sends a verification message to a standard administrative alias when DNS verification is not an option.",
        auth: "API key",
        endpoint: {
          method: "POST",
          url: "https://haveibeenpwned.com/api/v3/domainverification/sendemail",
          docsUrl: "https://haveibeenpwned.com/API/v3",
        },
        params: [
          { name: "domain", type: "string", required: true, desc: "The domain to verify." },
          { name: "emailAlias", type: "DomainVerificationEmailAlias", required: true, desc: "One of Admin, Hostmaster, Info, Security, or Webmaster." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task", desc: "Completes once the verification email is queued." },
        example: `await domains.SendDomainVerificationEmailAsync(
    "contoso.com",
    DomainVerificationEmailAlias.Security);

Console.WriteLine("Verification email sent to security@contoso.com.");`,
        output: `Verification email sent to security@contoso.com.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "domain is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status." },
        ],
        statusCodes: [
          { code: "200", meaning: "Email queued." },
          { code: "401", meaning: "Missing or invalid API key." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-breached-domain-async",
        name: "GetBreachedDomainAsync",
        signature: "Task<DomainBreaches?> GetBreachedDomainAsync(string domain, CancellationToken ct = default)",
        summary:
          "Returns every breached alias on a verified domain, mapped to the breaches each appeared in. Because control is proven, sensitive breaches are included.",
        auth: "API key + verified domain",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/breacheddomain/{domain}",
          docsUrl: "https://haveibeenpwned.com/API/v3#BreachesForDomain",
        },
        params: [
          { name: "domain", type: "string", required: true, desc: "A domain already verified on your dashboard." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<DomainBreaches?>", desc: "A dictionary of alias to breach names (DomainBreaches derives from Dictionary<string, string[]>), or null when nothing is found." },
        example: `DomainBreaches? results = await domains.GetBreachedDomainAsync("contoso.com");

foreach (var (alias, breaches) in results ?? [])
{
    Console.WriteLine($"{alias}@contoso.com: {string.Join(", ", breaches)}");
}`,
        output: `jsmith@contoso.com: Adobe, LinkedIn
mvargas@contoso.com: Dropbox`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "domain is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404." },
        ],
        statusCodes: [
          { code: "200", meaning: "Breached aliases returned." },
          { code: "401", meaning: "Missing or invalid API key." },
          { code: "403", meaning: "The domain is not verified for this key." },
          { code: "404", meaning: "No breached aliases. Returned as null." },
          commonRateLimit,
          serviceUnavailable,
        ],
        notes: ["The size of domain you can search is capped by your subscription's DomainSearchMaxBreachedAccounts."],
      },
      {
        id: "get-subscribed-domains-async",
        name: "GetSubscribedDomainsAsync",
        signature: "Task<SubscribedDomain[]> GetSubscribedDomainsAsync(CancellationToken ct = default)",
        summary: "Lists the domains associated with your API key, along with their last-known breach counts and renewal dates.",
        auth: "API key",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/subscribeddomains",
          docsUrl: "https://haveibeenpwned.com/API/v3#SubscribedDomains",
        },
        params: [{ name: "ct", type: "CancellationToken", desc: "Signals cancellation." }],
        returns: { type: "Task<SubscribedDomain[]>", desc: "Verified domains on the account, or an empty array." },
        streaming: "GetSubscribedDomainsAsAsyncEnumerable",
        example: `SubscribedDomain[] domainsOwned = await domains.GetSubscribedDomainsAsync();

foreach (var domain in domainsOwned)
{
    Console.WriteLine($"{domain.DomainName}: {domain.PwnCount:N0} pwned.");
}`,
        output: `contoso.com: 128 pwned.
fabrikam.com: 0 pwned.`,
        outputLang: "text",
        exceptions: [{ type: "PwnedApiException", when: "Any non-success status." }],
        statusCodes: [
          { code: "200", meaning: "Domains returned." },
          { code: "401", meaning: "Missing or invalid API key." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-stealer-logs-by-email-async",
        name: "GetStealerLogsByEmailAsync",
        signature: "Task<string[]?> GetStealerLogsByEmailAsync(string emailAddress, CancellationToken ct = default)",
        summary: "Returns the website domains where an email address was captured by an info stealer. The address must live on a verified domain.",
        auth: "API key + Pro subscription",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/stealerlogsbyemail/{emailAddress}",
          docsUrl: "https://haveibeenpwned.com/API/v3#StealerLogsByEmail",
        },
        params: [
          { name: "emailAddress", type: "string", required: true, desc: "The email address to search. Must be on a verified domain." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<string[]?>", desc: "Website domains, sorted alphabetically, or null when none are found." },
        example: `string[]? sites =
    await stealerLogs.GetStealerLogsByEmailAsync("jsmith@contoso.com");

Console.WriteLine(sites is { Length: > 0 }
    ? string.Join(", ", sites)
    : "No stealer-log exposure.");`,
        output: `github.com, netflix.com, okta.com`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "emailAddress is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404." },
        ],
        statusCodes: [
          { code: "200", meaning: "Stealer-log domains returned." },
          { code: "401", meaning: "Missing or invalid API key." },
          { code: "403", meaning: "The subscription does not include stealer logs, or the domain is not verified." },
          { code: "404", meaning: "No exposure found. Returned as null." },
          commonRateLimit,
          serviceUnavailable,
        ],
        notes: ["All stealer-log operations require a Pro subscription or higher (SubscriptionStatus.IncludesStealerLogs is true)."],
      },
      {
        id: "get-stealer-logs-by-website-domain-async",
        name: "GetStealerLogsByWebsiteDomainAsync",
        signature: "Task<string[]?> GetStealerLogsByWebsiteDomainAsync(string domain, CancellationToken ct = default)",
        summary: "Returns the email addresses captured against a website domain in stealer logs. Useful for operators triaging account-takeover risk.",
        auth: "API key + Pro subscription",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/stealerlogsbywebsitedomain/{domain}",
          docsUrl: "https://haveibeenpwned.com/API/v3#StealerLogsByWebsiteDomain",
        },
        params: [
          { name: "domain", type: "string", required: true, desc: "The website domain that appears in stealer-log URLs." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<string[]?>", desc: "Email addresses, sorted alphabetically, or null when none are found." },
        example: `string[]? accounts =
    await stealerLogs.GetStealerLogsByWebsiteDomainAsync("contoso.com");

Console.WriteLine($"{accounts?.Length ?? 0} accounts at risk.");`,
        output: `47 accounts at risk.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "domain is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404." },
        ],
        statusCodes: [
          { code: "200", meaning: "Stealer-log accounts returned." },
          { code: "401", meaning: "Missing or invalid API key." },
          { code: "403", meaning: "The subscription does not include stealer logs, or the domain is not verified." },
          { code: "404", meaning: "No exposure found. Returned as null." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
      {
        id: "get-stealer-logs-by-email-domain-async",
        name: "GetStealerLogsByEmailDomainAsync",
        signature: "Task<StealerLogsByEmailDomain?> GetStealerLogsByEmailDomainAsync(string domain, CancellationToken ct = default)",
        summary: "Maps each email alias on a verified domain to the website domains found in stealer logs. Ideal for organization-wide exposure reports.",
        auth: "API key + Pro subscription",
        endpoint: {
          method: "GET",
          url: "https://haveibeenpwned.com/api/v3/stealerlogsbyemaildomain/{domain}",
          docsUrl: "https://haveibeenpwned.com/API/v3#StealerLogsByEmailDomain",
        },
        params: [
          { name: "domain", type: "string", required: true, desc: "The email domain to search. Must be verified on your dashboard." },
          { name: "ct", type: "CancellationToken", desc: "Signals cancellation." },
        ],
        returns: { type: "Task<StealerLogsByEmailDomain?>", desc: "A dictionary of alias to website domains (derives from Dictionary<string, string[]>), or null when none are found." },
        example: `StealerLogsByEmailDomain? logs =
    await stealerLogs.GetStealerLogsByEmailDomainAsync("contoso.com");

foreach (var (alias, sites) in logs ?? [])
{
    Console.WriteLine($"{alias}: {sites.Length} sites.");
}`,
        output: `jsmith: 3 sites.
mvargas: 1 sites.`,
        outputLang: "text",
        exceptions: [
          { type: "ArgumentNullException", when: "domain is null, empty, or whitespace." },
          { type: "PwnedApiException", when: "Any non-success status other than 404." },
        ],
        statusCodes: [
          { code: "200", meaning: "Stealer-log map returned." },
          { code: "401", meaning: "Missing or invalid API key." },
          { code: "403", meaning: "The subscription does not include stealer logs, or the domain is not verified." },
          { code: "404", meaning: "No exposure found. Returned as null." },
          commonRateLimit,
          serviceUnavailable,
        ],
      },
    ],
  },

  telemetry: {
    title: "Telemetry",
    summary:
      "The client emits OpenTelemetry traces and metrics under a stable source name. Wire the ActivitySource and Meter into your existing pipeline to see request spans, durations, failures, and rate-limit hits.",
    icon: "ph-chart-line-up",
    clients: [
      { name: "PwnedClientTelemetry", desc: "Static class exposing the stable ActivitySource and Meter names, plus instrument names." },
    ],
    inject:
      "You do not inject PwnedClientTelemetry. Reference its constants when registering OpenTelemetry so the client's spans and metrics flow to your exporter.",
    registration: `using HaveIBeenPwned.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
        tracing.AddSource(PwnedClientTelemetry.ActivitySourceName))
    .WithMetrics(metrics =>
        metrics.AddMeter(PwnedClientTelemetry.MeterName));

var app = builder.Build();`,
    constants: [
      { name: "ActivitySourceName", value: "\"HaveIBeenPwned.Client\"", desc: "The stable ActivitySource name for client request spans." },
      { name: "MeterName", value: "\"HaveIBeenPwned.Client\"", desc: "The stable Meter name for the request and rate-limit instruments." },
    ],
    instruments: [
      { instrument: "hibp.client.request.count", kind: "Counter", unit: "{request}", desc: "Number of logical client requests issued." },
      { instrument: "hibp.client.request.duration", kind: "Histogram", unit: "s", desc: "Duration of each logical request in seconds." },
      { instrument: "hibp.client.request.failure.count", kind: "Counter", unit: "{request}", desc: "Number of requests that ended in a non-success response." },
      { instrument: "hibp.client.rate_limit.count", kind: "Counter", unit: "{response}", desc: "Number of HTTP 429 responses observed." },
    ],
    operations: [],
    closingExample: {
      label: "Sample metric export",
      code: `hibp.client.request.count{status="200"} 42
hibp.client.request.duration_bucket{le="0.25"} 39
hibp.client.request.failure.count{status="429"} 1
hibp.client.rate_limit.count 1`,
      outputLang: "text",
    },
  },
};
