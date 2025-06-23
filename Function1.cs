using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

namespace AzureFuncBe
{
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
    }
}
