using AzureFuncBe.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFuncBe.Controllers
{
    public class UserController
    {
        private readonly ILogger<FolderController> _logger;  
        private readonly UserService _userService;  
        public UserController(
            ILogger<FolderController> logger, 
            UserService userService
            )
        {
            _logger = logger;
            _userService = userService;
        }
        [Function("Login")]
        public async Task<IActionResult> Auth(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequest req)
        {
            if (string.IsNullOrEmpty(req.Headers["Authorization"].FirstOrDefault()))
            {
                _logger.LogWarning("Authorization header is missing or empty.");
                return new UnauthorizedResult();
            }
            try
            {
                await _userService.CreateIfUserNotExist(req);
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the request.");
                return new BadRequestObjectResult("An error occurred while processing the request.");
            }
        }

    }
}
