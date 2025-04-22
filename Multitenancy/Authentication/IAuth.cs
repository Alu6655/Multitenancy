using Multitenancy.Controllers;
using Multitenancy.Models;

namespace Multitenancy.Authentication
{
    public interface IAuth
    {
        string GenerateJwtToken(LoginTenant user);
        bool IsAuthorize(HttpRequest token, string? tenant);
    }
}
