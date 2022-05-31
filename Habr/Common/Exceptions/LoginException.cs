using Habr.Common.Exceptions.Base;

namespace Habr.Common.Exceptions;

public class LoginException : BaseException
{
    public LoginException(string message) : base(message, 401) {}
}