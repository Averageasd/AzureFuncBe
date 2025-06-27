using AzureFuncBe.Services;
using AzureFuncBe.Validations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFuncBe.Controllers
{
    public class UserController
    {
        private readonly ILogger<FolderController> _logger;  
        private readonly UserService _userService;
        private readonly UserValidation _userValidation;
        public UserController(
            ILogger<FolderController> logger, 
            UserService userService,
            UserValidation userValidation   
            )
        {
            _logger = logger;
            _userService = userService;
            _userValidation = userValidation;
        }
        [Function("Login")]
        public async Task<IActionResult> Auth(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "User/Login")]
        HttpRequest req)
        {
            if (!_userValidation.AuthHeaderExists(req))
            {
                return new UnauthorizedResult();
            }
            try
            {
                await _userService.CreateIfUserNotExist(req);
                return new OkResult();
            }
            catch (Exception)
            {
                return new BadRequestObjectResult("An error occurred while processing the request.");
            }
        }

        public async Task<IActionResult> GetUserProfile(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/Profile")]
            HttpRequest req
            )
        {

            return null;
        }
    }
}
