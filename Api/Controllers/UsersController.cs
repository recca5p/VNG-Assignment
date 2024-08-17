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
            var user = await _serviceManager.UserService.CreateAsync(request, cancellationToken);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Message = "User registered successfully.",
                Data = string.Empty
            });
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] UserSignInRequest request, CancellationToken cancellationToken)
        {
            var token = await _serviceManager.UserService.AuthenticateAsync(request, cancellationToken);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Sign-in successful.",
                Data = token
            });
        }
    }
}
