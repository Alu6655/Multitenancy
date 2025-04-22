using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Multitenancy.Authentication;
using Multitenancy.Context;
using Multitenancy.Finbuckle_Multitenant;
using Newtonsoft.Json;

namespace Multitenancy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuth _auth;
        private readonly AppTenantInfo _tenant;
        private readonly TenantContext _context;
        public WeatherForecastController(IConfiguration config, IAuth auth, IMultiTenantContextAccessor<AppTenantInfo> accessor, TenantContext context)
        {
            _config = config;
            _auth = auth;
            _tenant = accessor.MultiTenantContext?.TenantInfo;
            _context = context;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin request)
        {
            if (request != null)
            {
                var user = _context.LoginTenant.Where(x => x.LUserName == request.Username && x.LPassword == request.Password).FirstOrDefault();
                if (user != null)
                {
                    var token = _auth.GenerateJwtToken(user);
                    if (!string.IsNullOrEmpty(token))
                    {
                        ExtensionMethods.SetUserName(user.LUserName);
                        ExtensionMethods.SetIdentifier(user.LIdentifier);
                        return Ok(new { Success = true, Message = "Successfully logged in", Expiry = 3600, Token = token, Tenant = user.LIdentifier });
                    }
                    return Ok(new { Success = false, Message = "Unsuccesful login" });
                }
            }
            return Unauthorized();
        }
        public static string GenerateRandomEmail()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            string user = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            string domain = new string(Enumerable.Repeat(chars, 5)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return $"{user}@{domain}.com";
        }
        [HttpGet("profile")]
        public IActionResult GetProfile(int UserId)
        {
            if (!_auth.IsAuthorize(Request, ExtensionMethods.GetIdentifier()))
                return Unauthorized();
            var profile = _context.Users.FirstOrDefault(x => x.Id == UserId);
            return Ok(new { Success = true, Message = "Search user record", User = profile });
        }
    }
    #region common_models
    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Identifier { get; set; }
    }
    #endregion
}
