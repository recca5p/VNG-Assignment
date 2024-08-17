using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RepositoriyInterfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<User> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
        void Insert(User user);
        void Remove(User user);
        Task<User> GetUserByEmailAndPassword(User user, CancellationToken cancellationToken);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}
