using Contract.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.DTOs.Users
{
    public class UserModel
    {
        public string Email { get; set; }
        public UserStatus Status { get; set; }
    }
}
