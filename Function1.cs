using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

namespace AzureFuncBe
{
    public class Category
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("catName")]
        public string CatName { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("_ts")]
        public long Timestamp { get; set; }
    }

    public class CategoryParams
    {
        public string CatNameSearch { get; set; } = string.Empty;
        public int ProdCount { get; set; } = -1;

        public int MinProdCount { get; set; } = 0;
        public int MaxProdCount { get; set; } = 0;
    }
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly IConfiguration _configuration;
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseName;
        private readonly string _categoryContainer;
        private readonly string _itemContainer;


        public Function1(CosmosClient cosmoClient, ILogger<Function1> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _cosmosClient = cosmoClient;
            _databaseName = _configuration["CosmosDb:DatabaseName"]!;
            _categoryContainer = _configuration["CategoryContainerName"]!;
            _itemContainer = _configuration["ItemsContainerName"]!;
        }

        private Container GetContainer(string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentException("Container name cannot be null or empty.", nameof(containerName));
            }
            return _cosmosClient.GetContainer(_databaseName, containerName);
        }

        [Function("Login")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
        HttpRequest req)
        {
            if (string.IsNullOrEmpty(req.Headers["Authorization"].FirstOrDefault()))
            {
                _logger.LogWarning("Authorization header is missing or empty.");
                return new UnauthorizedResult();
            }
            string token = req.Headers["Authorization"].FirstOrDefault()!.Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            try
            {
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                string? subject = jwtToken?.Subject;
                if (subject == null)
                {
                    throw new InvalidOperationException("token not found");
                }
                Container container = GetContainer(_categoryContainer);
                Container itemContainer = GetContainer(_itemContainer);
                return new OkObjectResult(new
                {
                    subject,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid JWT token.");
                return new BadRequestObjectResult("Invalid JWT token.");
            }
        }

        private CategoryParams ConstructCategoryQueryParams(HttpRequest req)
        {
            string catNameSuggestion = req.Query["catNameSearch"].ToString();
            int minProdCount = int.TryParse(req.Query["minProdCount"], out var min) ? min : 0;
            int maxProdCount = int.TryParse(req.Query["maxProdCount"], out var max) ? max : 0;
            int prodCount = int.TryParse(req.Query["prodCount"], out var count) ? count : -1;
            return new CategoryParams
            {
                CatNameSearch = catNameSuggestion,
                MinProdCount = minProdCount,
                MaxProdCount = maxProdCount,
                ProdCount = prodCount
            };
        }

        [Function("GetPartitionedCategory")]
        public async Task<IActionResult> GetPartitionedCategory(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post")]
        HttpRequest req
    )
        {
            CategoryParams categoryParams = ConstructCategoryQueryParams(req);
            string catNameSuggestion = categoryParams.CatNameSearch;
            int minProdCount = categoryParams.MinProdCount;
            int maxProdCount = categoryParams.MaxProdCount;
            int prodCount = categoryParams.ProdCount;
            Container container = GetContainer(_categoryContainer);
            string query = "SELECT * FROM Category c WHERE c.catName LIKE @catNameSuggestion";

            if (categoryParams.ProdCount == -1)
            {
                if (minProdCount == maxProdCount && minProdCount == 0)
                {
                    query += " AND c.count >= @minProdCount";
                }
                else
                {
                    query += " AND c.count >= @minProdCount AND c.count <= @maxProdCount";
                }
            }

            else if (categoryParams.ProdCount != -1)
            {
                query += " AND c.count = @prodCount";
            }
            var queryDefinition = new QueryDefinition(query);
            queryDefinition
                .WithParameter("@catNameSuggestion", $"%{catNameSuggestion}%")
                .WithParameter("@maxProdCount", maxProdCount)
                .WithParameter("@minProdCount", minProdCount)
                .WithParameter("@prodCount", prodCount);
            string continuationToken = null;
            List<Category> categories = new List<Category>();
            QueryRequestOptions queryRequest = new QueryRequestOptions
            {
                MaxItemCount = 10
            };
            FeedIterator<Category> feedIterator = container.GetItemQueryIterator<Category>(queryDefinition, continuationToken, queryRequest);
            FeedResponse<Category> feedResponse = await feedIterator.ReadNextAsync();
            categories.AddRange(feedResponse);

            return new OkObjectResult(new
            {
                categories,
                continuationToken
            });
        }
    }
}
