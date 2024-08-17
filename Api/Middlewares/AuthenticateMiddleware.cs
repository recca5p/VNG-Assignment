using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Contract.Helpers;
using Contract.Models;
using Microsoft.Extensions.Options;
using Services;
using Services.Abstractions;

public class AuthenticateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public AuthenticateMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_appSettings.ExcludedPaths.Contains(context.Request.Path.Value.ToLower()))
        {
            await _next(context);
            return;
        }

        var token = context.Request.Headers["xAuth"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            try
            {
                var principal = TokenService.ValidateToken(token, _appSettings.JwtSettings.Key);

                if (principal != null)
                {
                    context.User = principal;
                    await _next(context);
                    return;
                }
            }
            catch
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
        }
        throw new UnauthorizedAccessException("Invalid token");
    }
}