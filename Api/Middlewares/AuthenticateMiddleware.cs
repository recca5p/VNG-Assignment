using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Contract.DTOs.Users;
using Contract.Enum;
using Contract.Helpers;
using Contract.Models;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Services;
using Services.Abstractions;

public class AuthenticateMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuthenticateMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings, IServiceScopeFactory serviceScopeFactory)
    {
        _next = next;
        _appSettings = appSettings.Value;
        _serviceScopeFactory = serviceScopeFactory;
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
                var isValid = TokenService.ValidateToken(token, _appSettings.JwtSettings.Key);

                if (isValid)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var serviceManager = scope.ServiceProvider.GetRequiredService<IServiceManager>();

                        Guid userId = TokenService.GetUserIdFromToken(token, _appSettings.JwtSettings.Key);

                        UserModel user = await serviceManager.UserService.GetByIdAsync(userId);

                        if (user.Status != UserStatus.Verified)
                            throw new UnauthorizedAccessException("Invalid token");
                    }
                }
            }
            catch
            {
                throw new UnauthorizedAccessException("Invalid token");
            }
        }
        await _next(context);
    }
}