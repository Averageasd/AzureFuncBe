using AzureFuncBe.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace AzureFuncBe.Services
{
    public class JWTTokenDecoder
    {
        public JwtSecurityToken ValidateAndReturnToken(HttpRequest req)
        {
            string token = req.Headers["Authorization"].FirstOrDefault()!.Split(" ")[1];
            var handler = new JwtSecurityTokenHandler();
            try
            {
                JwtSecurityToken jwtToken = handler.ReadJwtToken(token);
                return jwtToken;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public AuthenticatedUserDTO GetAuthenticatedUserDTO(JwtSecurityToken jwtToken)
        {

            string? subject = jwtToken?.Subject;
            string emailClaim = jwtToken!.Claims.FirstOrDefault(c => c.Type == "preferred_username")!.Value;
            if (subject == null)
            {
                throw new InvalidOperationException("token not found");
            }
            return new AuthenticatedUserDTO
            {
                Id = subject,
                Email = emailClaim
            };
        }
    }
}
