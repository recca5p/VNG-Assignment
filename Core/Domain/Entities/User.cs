using Contract.Enum;
using Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class User : AuditEntity<Guid>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public UserStatus Status { get; set; }
        
        public User() {}

        public User(Guid id, string email, string password, UserStatus status)
        {
            Id = id;
            Email = email;
            Password = password;
            Status = status;
        }
    }
}
