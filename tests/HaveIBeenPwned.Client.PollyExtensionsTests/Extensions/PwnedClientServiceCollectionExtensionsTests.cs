// Copyright (c) David Pine. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using HaveIBeenPwned.Client.Http;
using HaveIBeenPwned.Client.Options;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Xunit;

namespace HaveIBeenPwned.Client.PollyExtensionsTests.Extensions;

public class PwnedClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPwnedServicesThrowsWhenServiceCollectionIsNull() =>
    Assert.Throws<ArgumentNullException>(
        "services",
        () => ((IServiceCollection)null!).AddPwnedServices(_ => { }));

    [Fact]
    public void AddPwnedServicesThrowsWhenConfigureOptionsIsNull() =>
        Assert.Throws<ArgumentNullException>(
            "configureOptions",
            () => new ServiceCollection().AddPwnedServices(
                (null as Action<HibpOptions>)!));

    [Fact]
    public void AddPwnedServicesThrowsWhenConfigureRetryPolicyIsNull() =>
        Assert.Throws<ArgumentNullException>(
            "configureRetryPolicy",
            () => new ServiceCollection().AddPwnedServices(
                options => { },
                (null as Func<PolicyBuilder<HttpResponseMessage>, IAsyncPolicy<HttpResponseMessage>>)!));

    [Fact]
    public void AddPwnedServicesAddsDefaultImplementationsWhenValid()
    {
        static IAsyncPolicy<HttpResponseMessage> TestPolicyFactory(PolicyBuilder<HttpResponseMessage> _) => new TestPolicy();

        var services = new ServiceCollection()
            .AddPwnedServices(options => { }, TestPolicyFactory)
            .BuildServiceProvider();

        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedBreachesClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedPasswordsClient>().GetType());
        Assert.Equal(typeof(DefaultPwnedClient), services.GetRequiredService<IPwnedPastesClient>().GetType());

        var httpClientFactory = services.GetRequiredService<IHttpClientFactory>();
        var http = httpClientFactory.CreateClient(HttpClientNames.HibpClient);
        Assert.NotNull(http);

        // Yuck, but at least we can verify this is configured correctly.
        var policy =
            http.ReflectNestedPrivateField<IAsyncPolicy<HttpResponseMessage>>(
                "_handler", "_innerHandler", "_innerHandler", "_policy");

        Assert.NotNull(policy);
        Assert.Equal("Testing policy configuration", policy!.PolicyKey);
    }

    class TestPolicy : IAsyncPolicy<HttpResponseMessage>
    {
        string IsPolicy.PolicyKey => "Testing policy configuration";

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<Task<HttpResponseMessage>> action)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<Context, Task<HttpResponseMessage>> action, IDictionary<string, object> contextData)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<Context, Task<HttpResponseMessage>> action, Context context)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<CancellationToken, Task<HttpResponseMessage>> action, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, Context context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<CancellationToken, Task<HttpResponseMessage>> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }

        Task<PolicyResult<HttpResponseMessage>> IAsyncPolicy<HttpResponseMessage>.ExecuteAndCaptureAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<Task<HttpResponseMessage>> action)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<Context, Task<HttpResponseMessage>> action, Context context)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<Context, Task<HttpResponseMessage>> action, IDictionary<string, object> contextData)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<CancellationToken, Task<HttpResponseMessage>> action, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, Context context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<CancellationToken, Task<HttpResponseMessage>> action, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, IDictionary<string, object> contextData, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }

        Task<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.ExecuteAsync(Func<Context, CancellationToken, Task<HttpResponseMessage>> action, Context context, CancellationToken cancellationToken, bool continueOnCapturedContext)
        {
            throw new NotImplementedException();
        }

        IAsyncPolicy<HttpResponseMessage> IAsyncPolicy<HttpResponseMessage>.WithPolicyKey(string policyKey)
        {
            throw new NotImplementedException();
        }
    }
}

static class ObjectExtensions
{
    internal static T? ReflectNestedPrivateField<T>(
        this object parentObject, params string[] fieldNames) where T : class
    {
        object? interimObject = null;
        foreach (var (fieldName, index) in
            fieldNames.Select((value, index) => (value, index)))
        {
            interimObject = interimObject is null
                ? parentObject.GetFieldValue(fieldName, true)
                : interimObject.GetFieldValue(fieldName, index + 1 < fieldNames.Length);
        }

        return interimObject as T;
    }

    public static object? GetFieldValue(this object obj, string name, bool useBaseClass = false)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var type = (useBaseClass ? obj.GetType().BaseType : obj.GetType())!;
        var field = type.GetField(name, bindingFlags);
        return field?.GetValue(obj);
    }
}
