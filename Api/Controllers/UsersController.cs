using Contract.DTOs.Users;
using Contract.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Abstractions;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/owners")]
    public class UsersController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public UsersController(IServiceManager serviceManager) => _serviceManager = serviceManager;

        [HttpGet]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            var users = await _serviceManager.UserService.GetAllAsync(cancellationToken);

            ApiResponse<IEnumerable<UserModel>> response = new ApiResponse<IEnumerable<UserModel>>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = users
            };

            return Ok(response);
        }

        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
        {
            var ownerDto = await _serviceManager.UserService.GetByIdAsync(userId, cancellationToken);

            ApiResponse<UserModel> response = new ApiResponse<UserModel>()
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Success",
                Data = ownerDto
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request)
        {
            var user = await _serviceManager.UserService.CreateAsync(request);

            return NoContent();
        }

        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateRequest request, CancellationToken cancellationToken)
        {
            await _serviceManager.UserService.UpdateAsync(userId, request, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteOwner(Guid userId, CancellationToken cancellationToken)
        {
            await _serviceManager.UserService.DeleteAsync(userId, cancellationToken);

            return NoContent();
        }
    }
}
