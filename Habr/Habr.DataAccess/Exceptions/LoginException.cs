namespace Habr.DataAccess;

public class LoginException : Exception
{
    public LoginException(string message) : base(message) {}
}