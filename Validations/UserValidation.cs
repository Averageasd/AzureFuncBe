using Microsoft.AspNetCore.Http;

namespace AzureFuncBe.Validations
{
    public class UserValidation
    {
        public bool AuthHeaderExists(HttpRequest req)
        {
            if (req.Headers.ContainsKey("Authorization") &&
                !string.IsNullOrEmpty(req.Headers["Authorization"].FirstOrDefault()))
            {
                return true;
            }
            return false;
        }
    }
}
