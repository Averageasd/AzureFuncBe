using AzureFuncBe.ContainerManager;
using AzureFuncBe.DTOs;
using AzureFuncBe.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

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

        public async Task CreateIfUserNotExist(HttpRequest req)
        {
            AuthenticatedUserDTO authenticatedUserDTO = _jwtTokenDecoder.GetAuthenticatedUserDTO(req);
            var userWithId = await GetUserById(authenticatedUserDTO.Id);
            if (userWithId == null)
            {
                var userContainer = _dBContainerManager.GetContainer(_dBContainerManager.GetUserContainerName());
                UserModel user = new UserModel
                {
                    Id = authenticatedUserDTO.Id,
                    Name = authenticatedUserDTO.Email!,
                };
                await userContainer.CreateItemAsync(user, new PartitionKey(user.Id));
            }

        }

        public async Task<UserModel> GetUserById(string id)
        {
            try
            {
                var userContainer = _dBContainerManager.GetContainer(_dBContainerManager.GetUserContainerName());
                using ResponseMessage response = await userContainer.ReadItemStreamAsync(id, new PartitionKey(id));
                if (response.IsSuccessStatusCode)
                {
                    using StreamReader sr = new StreamReader(response.Content);
                    string json = await sr.ReadToEndAsync();
                    return JsonSerializer.Deserialize<UserModel>(json)!;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw;
            }
           
        }
    }
}
