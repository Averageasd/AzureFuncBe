using AzureFuncBe.ContainerManager;
using AzureFuncBe.DTOs;
using AzureFuncBe.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace AzureFuncBe.Services
{
    public class UserService
    {
        private readonly JWTTokenDecoder _jwtTokenDecoder;
        private readonly DBContainerManager _dBContainerManager;
        public UserService
            (
            JWTTokenDecoder jwtTokenDecoder,
            DBContainerManager dBContainerManager
            )
        {
            _jwtTokenDecoder = jwtTokenDecoder;
            _dBContainerManager = dBContainerManager;
        }

        public async Task<AuthenticatedUserDTO> CreateIfUserNotExist(JwtSecurityToken jwtSecurityToken)
        {
            try
            {
                AuthenticatedUserDTO authenticatedUserDTO = _jwtTokenDecoder.GetAuthenticatedUserDTO(jwtSecurityToken);
                var userWithId = await GetUserById(authenticatedUserDTO.Id!);
                if (userWithId == null)
                {
                    var userContainer = _dBContainerManager.GetContainer(_dBContainerManager.GetUserContainerName());
                    UserModel user = new UserModel
                    {
                        Id = authenticatedUserDTO.Id!,
                        Name = authenticatedUserDTO.Email!,
                        Profile = new ProfileModel()
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserId = authenticatedUserDTO.Id,
                        }
                    };
                    await userContainer.CreateItemAsync(user, new PartitionKey(user.Id));
                }
                return authenticatedUserDTO;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<UserModel> GetProfile(string userId)
        {
            try
            {
                var userContainer = _dBContainerManager.GetContainer(_dBContainerManager.GetUserContainerName());
                var user = await GetUserById(userId);
                return user ?? throw new InvalidOperationException("User not found");
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<UserModel?> GetUserById(string id)
        {
            try
            {
                var userContainer = _dBContainerManager.GetContainer(_dBContainerManager.GetUserContainerName());
                using ResponseMessage response = await userContainer.ReadItemStreamAsync(id, new PartitionKey(id));
                if (response.IsSuccessStatusCode)
                {
                    using StreamReader sr = new StreamReader(response.Content);
                    string json = await sr.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<UserModel>(json)!;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
