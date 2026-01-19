using Common.Clients.DependencyInjection;

using Common.Constants;

using IntegrationTests.Infrastructure.Core;
using IntegrationTests.Mocks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Xunit;

namespace IntegrationTests.WebApplicationFactories;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IConfiguration? _testConfiguration;
    private TestContainers? _testContainers;

    async Task IAsyncLifetime.InitializeAsync()
    {
        _testConfiguration = BuildTestConfiguration();

        _testContainers = new TestContainers(_testConfiguration);
        await _testContainers.InitializeAsync();

        CreateClient();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        if (_testContainers is not null)
        {
            await _testContainers.DisposeAsync();
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(EnvironmentConstants.Test);

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            if (_testConfiguration is not null)
            {
                configurationBuilder.AddConfiguration(_testConfiguration);
            }
            if (_testContainers is not null)
            {
                configurationBuilder.AddInMemoryCollection(_testContainers.BuildAppsettingsOverrides());
            }
        });

        builder.ConfigureServices((context, services) =>
        {
            _testContainers!.RegisterServices(services);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthenticationMock.SchemeName;
                options.DefaultChallengeScheme = AuthenticationMock.SchemeName;
            })
            .AddScheme<AuthenticationSchemeOptions, AuthenticationMock>(AuthenticationMock.SchemeName, _ => { });

            services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder(AuthenticationMock.SchemeName)
                    .RequireAuthenticatedUser()
                    .Build());

            services.UseHttpClients(context.Configuration);
            services.AddHttpClient(ClientsConstants.ApiClient).ConfigurePrimaryHttpMessageHandler(_ => Server.CreateHandler());
        });
    }

    private static IConfiguration BuildTestConfiguration()
    {
        var testJson = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        return new ConfigurationBuilder()
            .AddJsonFile(testJson, optional: false, reloadOnChange: false)
            .Build();
    }
}
