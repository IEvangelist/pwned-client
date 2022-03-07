# ![';-- have i been pwned? — .NET HTTP client.](https://raw.githubusercontent.com/IEvangelist/pwned-client/main/assets/pwned-header.png)

[![build](https://github.com/IEvangelist/pwned-client/actions/workflows/build-validation.yml/badge.svg)](https://github.com/IEvangelist/pwned-client/actions/workflows/build-validation.yml) [![code analysis](https://github.com/IEvangelist/pwned-client/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/IEvangelist/pwned-client/actions/workflows/codeql-analysis.yml) [![NuGet](https://img.shields.io/nuget/v/HaveIBeenPwned.Client.svg?style=flat)](https://www.nuget.org/packages/HaveIBeenPwned.Client)

A .NET HTTP client for the "have i been pwned" API service from Troy Hunt. This library is comprised of three NuGet packages:

- [`HaveIBeenPwned.Client`](https://www.nuget.org/packages/HaveIBeenPwned.Client)
- [`HaveIBeenPwned.Client.Abstractions`](https://www.nuget.org/packages/HaveIBeenPwned.Client.Abstractions)
- [`HaveIBeenPwned.Client.PollyExtensions`](https://www.nuget.org/packages/HaveIBeenPwned.Client.PollyExtensions)

> Consumers of the API can use the abstractions for the models returned from the API, while server APIs can consume and wrap the client.

## Getting started

Install from the .NET CLI:

```shell
dotnet add package HaveIBeenPwned.Client
```

Alternatively add manually to your consuming _.csproj_:

```xml
<PackageReference Include="HaveIBeenPwned.Client" Version="{VersionNumber}" />
```

Or, install using the NuGet Package Manager:

```powershell
Install-Package HaveIBeenPwned.Client
```

### Dependency injection

To add all of the services to the dependency injection container, call one of the `AddPwnedServices` overloads. From Minimal APIs for example, with using a named configuration section:

```csharp
builder.Services.AddPwnedServices(
    builder.Configuration.GetSection(nameof(HibpOptions)));
```

From a `ConfigureServices` method, with an `IConfiguration` instance:

```csharp
services.AddPwnedServices(options =>
    {
        options.ApiKey = _configuration["HibpOptions:ApiKey"];
        options.UserAgent = _configuration["HibpOptions:UserAgent"];
    });
```

Then you can require any of the available DI-ready types:

- `IPwnedBreachesClient`: [Breaches API](https://haveibeenpwned.com/API/v3#BreachesForAccount).
- `IPwnedPastesClient`: [Pastes API](https://haveibeenpwned.com/API/v3#PastesForAccount).
- `IPwnedPasswordsClient`: [Pwned Passwords API](https://haveibeenpwned.com/API/v3#PwnedPasswords).
- `IPwnedClient`: Marker interface, for conveniently injecting all of the above clients into a single client.

### Example Minimal APIs

![Minimal APIs example code.](https://raw.githubusercontent.com/IEvangelist/pwned-client/main/assets/minimal-api.svg)

## Configuration

To configure the `HaveIBeenPwned.Client`, the following table identifies the well-known configuration object:

### Well-known keys

Depending on the [.NET configuration provider](https://docs.microsoft.com/dotnet/core/extensions/configuration-providers?WC.m_id=dapine) your app is using, there are several well-known keys that map to the `HibpOptions` that configure your usage of the HTTP client. When using environment variables, such as those in Azure App Service configuration or Azure Key Vault secrets, the following keys map to the `HibpOption` instance:

| Key                      | Data type | Default value                              |
|--------------------------|-----------|--------------------------------------------|
| `HibpOptions__ApiKey`    | `string`  | `null`                                     |
| `HibpOptions__UserAgent` | `string`  | `".NET HIBP Client/{AssemblyFileVersion}"` |

The `ApiKey` is required, to get one &mdash; sign up here: <https://haveibeenpwned.com/api/key>

### Example `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "HibpOptions": {
    "ApiKey": "<YourApiKey>",
    "UserAgent": "<YourUserAgent>"
  }
}
```

For more information, see [JSON configuration provider](https://docs.microsoft.com/dotnet/core/extensions/configuration-providers?WC.m_id=dapine#json-configuration-provider).

<!--
Notes for tagging releases:
  https://rehansaeed.com/the-easiest-way-to-version-nuget-packages/#minver

git tag -a 0.0.3 -m "Build version 0.0.3"
git push upstream --tags
-->
