using AchievementsPlatform.Services.Auth.Interfaces;

namespace AchievementsPlatform.Middleware
{
    public class TokenRevocationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenRevocationService _revocationService;

        public TokenRevocationMiddleware(RequestDelegate next, ITokenRevocationService revocationService)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _revocationService = revocationService ?? throw new ArgumentNullException(nameof(revocationService));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                var isRevoked = await _revocationService.IsTokenRevokedAsync(token);
                if (isRevoked)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Token revogado.");
                    return;
                }
            }

            await _next(context);
        }
    }
}