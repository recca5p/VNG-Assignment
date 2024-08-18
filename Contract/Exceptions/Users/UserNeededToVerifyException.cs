namespace Contract.Exceptions.Users;

public class UserNeededToVerifyException : BadRequestException
{
    public UserNeededToVerifyException(string email)
        : base($"The user with the email {email} is needed to verify")
    {
    }
}