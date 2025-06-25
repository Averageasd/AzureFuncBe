
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostContext, config) => 
    {
        config.SetBasePath(Environment.CurrentDirectory);
        config.AddEnvironmentVariables(); 
    })
    .ConfigureServices((hostContext, services) =>
    {
        // Get configuration settings directly from environment variables
        var configuration = hostContext.Configuration;

        string databaseName = configuration["CosmosDb:DatabaseName"]
                            ?? throw new InvalidOperationException("CosmosDb:DatabaseName environment variable is not set.");

        string accountEndpoint = configuration["CosmosDbConnection"]
                               ?? throw new InvalidOperationException("CosmosDbConnection environment variable is not set.");

        services.AddSingleton<CosmosClient>(s =>
        {
            var client = new CosmosClient(accountEndpoint);
            return client;
        });
    })
    .Build();

host.Run();