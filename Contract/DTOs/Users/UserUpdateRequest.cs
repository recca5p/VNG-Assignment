using System.ComponentModel.DataAnnotations;

namespace Contract.DTOs.Users;

public class UserUpdateRequest
{
    [Required]
    public string Password { get; set; }
}