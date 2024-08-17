namespace Contract.Exceptions.Users;

public class UserExistException: BadRequestException
{
    public UserExistException(string email)
        : base($"The user with the emai {email} is existed")
    {
    }
}