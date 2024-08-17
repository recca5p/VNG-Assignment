using Contract.DTOs.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstractions
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserModel> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserModel> CreateAsync(UserCreateRequest user, CancellationToken cancellationToken = default);
        Task UpdateAsync(Guid ownerId, UserUpdateRequest user, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid ownerId, CancellationToken cancellationToken = default);
        Task<string> AuthenticateAsync(UserSignInRequest request, CancellationToken cancellationToken = default);
        ClaimsPrincipal ValidateToken(string token);
    }
}
