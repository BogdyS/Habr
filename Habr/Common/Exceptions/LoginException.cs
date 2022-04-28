namespace Habr.Common.Exceptions;

public class LoginException : Exception
{
    public LoginException(string message) : base(message) {}
}