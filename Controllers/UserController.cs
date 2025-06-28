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
        private readonly JWTTokenDecoder _jWTTokenDecoder;
        public UserController(
            ILogger<FolderController> logger, 
            UserService userService,
            JWTTokenDecoder jWTTokenDecoder,
            UserValidation userValidation   
            )
        {
            _logger = logger;
            _userService = userService;
            _userValidation = userValidation;
            _jWTTokenDecoder = jWTTokenDecoder;
        }
        [Function("Auth")]
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
                var token = _jWTTokenDecoder.ValidateAndReturnToken(req);
                var authenticatedUserDTO = await _userService.CreateIfUserNotExist(token);
                return new OkObjectResult(authenticatedUserDTO);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [Function("GetUserProfile")]
        public async Task<IActionResult> GetUserProfile(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "User/Profile/{userId}")]
            HttpRequest req,
            string userId
            )
        {
            if (!_userValidation.AuthHeaderExists(req))
            {
                return new UnauthorizedResult();
            }
            try
            {
                var userProfile = await _userService.GetProfile(userId);    
                return new OkObjectResult(userProfile);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
