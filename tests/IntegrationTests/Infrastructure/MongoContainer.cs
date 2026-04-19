using Common.Constants;

using Microsoft.Extensions.Configuration;

using MongoDB.Driver;

using Testcontainers.MongoDb;

namespace IntegrationTests.Infrastructure;

public sealed class MongoContainer
{
    private readonly MongoDbContainer _mongoDbContainer;

    private readonly string _databaseName;
    private readonly string _username;
    private readonly string _password;

    public MongoContainer()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        var mongoUrl = new MongoUrl(configuration.GetConnectionString(AppsettingsKeysConstants.MongoDbConnectionString));

        var mongoImage = configuration["Testcontainers:MongoImage"] ?? "mongo:4.4.29";

        _databaseName = mongoUrl.DatabaseName;
        _username = mongoUrl.Username;
        _password = mongoUrl.Password;

        _mongoDbContainer = new MongoDbBuilder(mongoImage)
            .WithUsername(_username)
            .WithPassword(_password)
            .WithCleanUp(true)
            .Build();
    }

    public string ConnectionString
    {
        get
        {
            var builder = new MongoUrlBuilder(_mongoDbContainer.GetConnectionString())
            {
                DatabaseName = _databaseName,
                Username = _username,
                Password = _password,
                AuthenticationSource = "admin",
            };

            return builder.ToString();
        }
    }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mongoDbContainer.DisposeAsync().AsTask();
    }

    public async Task ResetDatabaseAsync()
    {
        var client = new MongoClient(ConnectionString);
        await client.DropDatabaseAsync(_databaseName);
    }
}
