namespace Contract.DTOs.Users;

public class UserSignInResponse
{
    public string Token { get; set; }
    public Guid UserId { get; set; }
}