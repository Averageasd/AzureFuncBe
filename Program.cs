using Azure.Storage.Blobs;
using AzureFuncBe.ContainerManager;
using AzureFuncBe.Services;
using AzureFuncBe.Validations;
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
        string fcBlobEndpoint = configuration["AzureBlobStorageConnection"]
                                ?? throw new InvalidOperationException
                                ("AzureBlobStorageConnection environment variable is not set.");

        var cosmosClientOptions = new CosmosClientOptions
        {
            AllowBulkExecution = true,
            MaxRetryAttemptsOnRateLimitedRequests = 10
        };
        services.AddSingleton<CosmosClient>(s =>
        {
            var client = new CosmosClient(accountEndpoint, cosmosClientOptions);
            return client;
        });

        services.AddSingleton<BlobServiceClient>(s =>
        {
            return new BlobServiceClient(fcBlobEndpoint);
        });

        services.AddScoped<DBContainerManager>();
        services.AddScoped<UserService>();
        services.AddScoped<FolderService>();
        services.AddScoped<FlashcardService>();
        services.AddScoped<JWTTokenDecoder>();
        services.AddScoped<UserValidation>();
        services.AddScoped<QueueStorageManager>();
        services.AddScoped<BlobContainerManager>();
    })
    .Build();

host.Run();