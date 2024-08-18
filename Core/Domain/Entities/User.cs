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
        public ICollection<Book> Books = new HashSet<Book>();
    }
}
