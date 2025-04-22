using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Multitenancy
{
    public static class ExtensionMethods
    {
        private static string LoginUserName = string.Empty;
        private static string LoginIdentifier = string.Empty;
        public static string? GetUsername()
        {
            return LoginUserName;
        }

        public static string? GetIdentifier()
        {
            return LoginIdentifier;
        }
        public static void SetUserName(string username) => LoginUserName = username;
        public static void SetIdentifier(string identifier) => LoginIdentifier = identifier;
    }
}
