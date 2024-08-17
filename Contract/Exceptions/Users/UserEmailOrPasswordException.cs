namespace Contract.Exceptions.Users;

public class UserEmailOrPasswordException: NotFoundException
{
    public UserEmailOrPasswordException()
        : base($"The user email or password is not correct.")
    {
    }
}