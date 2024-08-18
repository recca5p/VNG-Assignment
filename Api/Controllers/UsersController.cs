using System.Security.Claims;
using Amazon.Auth.AccessControlPolicy;
using Contract.DTOs.Users;
using Contract.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AuthController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserCreateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _serviceManager.UserService.CreateAsync(request, cancellationToken);

                return Ok(new ApiResponse<string>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "User registered successfully.",
                    Data = string.Empty
                });
            }
            catch (Exception e)
            {
                if (e.Message.Contains("Failed to send"))
                {
                    return Ok(new ApiResponse<string>
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status201Created,
                        Message = e.Message,
                        Data = string.Empty
                    });
                }

                throw;
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] UserSignInRequest request, CancellationToken cancellationToken)
        {
            var (token, userId) = await _serviceManager.UserService.AuthenticateAsync(request, cancellationToken);

            return Ok(new ApiResponse<UserSignInResponse>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Sign-in successful.",
                Data = new UserSignInResponse ()
                {
                    Token = token,
                    UserId = userId
                }
            });
        }
        
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token, CancellationToken cancellationToken = default)
        {
            if (token is null)
                throw new UnauthorizedAccessException("Token is null");
            
            await _serviceManager.UserService.ValidateUser(token);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Email verified successfully",
                Data = string.Empty
            });
        }
    }
}
