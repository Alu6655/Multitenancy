using Microsoft.IdentityModel.Tokens;
using Multitenancy.Controllers;
using Multitenancy.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Multitenancy.Authentication
{
    public class Auth : IAuth
    {
        private IConfiguration _config;
        public Auth(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateJwtToken(LoginTenant user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Name, user.LUserName!),
            new Claim(JwtRegisteredClaimNames.Jti, user.LIdentifier!)
            };
            var securityKey = new SymmetricSecurityKey(Convert.FromBase64String(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            null,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool IsAuthorize(HttpRequest request, string? tenant)
        {
            try
            {
                string? token = null;
                if (request.Headers.ContainsKey("Authorization") && request.Headers.ContainsKey("Tenant"))
                {
                    if (request.Headers["Tenant"] != tenant)
                        return false;
                    var bearer = request.Headers["Authorization"].ToString();
                    if (bearer.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        token = bearer.Substring("Bearer ".Length).Trim();
                    else
                        token = bearer;
                    if (token is not null)
                    {
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var key = Convert.FromBase64String(_config["Jwt:Key"]);

                        tokenHandler.ValidateToken(token, new TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(key),
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidIssuer = _config["Jwt:Issuer"],
                            ValidAudience = _config["Jwt:Issuer"],
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero // exact expiration time
                        }, out SecurityToken validatedToken);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }
    }
}
