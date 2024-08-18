namespace Contract.Exceptions.Users;

public class UserNeededToChangePasswordException : BadRequestException
{
    public UserNeededToChangePasswordException(string email)
        : base($"The user with the email {email} is needed to change password")
    {
    }
}