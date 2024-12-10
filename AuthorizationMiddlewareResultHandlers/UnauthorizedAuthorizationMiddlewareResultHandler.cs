using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace SimpleHelpDeskAPI.AuthorizationMiddlewareResultHandlers
{
    public class UnauthorizedAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultAuthorizationMiddlewareResultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            if (!authorizeResult.Succeeded)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            await _defaultAuthorizationMiddlewareResultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}