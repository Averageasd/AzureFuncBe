using AzureFuncBe.ContainerManager;
using AzureFuncBe.Services;
using AzureFuncBe.Validations;
using Microsoft.AspNetCore.Mvc.ViewComponents;
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
        var configuration = hostContext.Configuration;

        string databaseName = configuration["CosmosDb:DatabaseName"]
                            ?? throw new InvalidOperationException("CosmosDb:DatabaseName environment variable is not set.");

        string accountEndpoint = configuration["CosmosDbConnection"]
                               ?? throw new InvalidOperationException("CosmosDbConnection environment variable is not set.");

        var cosmosClientOptions = new CosmosClientOptions
        {
            AllowBulkExecution = true
        };
        services.AddSingleton<CosmosClient>(s =>
        {
            var client = new CosmosClient(accountEndpoint, cosmosClientOptions);
            return client;
        });

        services.AddScoped<DBContainerManager>();
        services.AddScoped<UserService>();
        services.AddScoped<FolderService>();
        services.AddScoped<FlashcardService>();
        services.AddScoped<JWTTokenDecoder>();
        services.AddScoped<UserValidation>();
    })
    .Build();

host.Run();