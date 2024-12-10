using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SimpleHelpDeskAPI.Utilities
{
    public class AuthenticationUtility
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationUtility(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SignInAsync(string role, string id)
        {
            var claims = new List<Claim>
            {
                new("Role", role),
                new("ID", id)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _httpContextAccessor.HttpContext!.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new(claimsIdentity));
        }
    }
}