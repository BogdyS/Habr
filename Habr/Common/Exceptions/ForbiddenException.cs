using Habr.Common.Exceptions.Base;

namespace Habr.Common.Exceptions;

public class ForbiddenException : BaseException
{
    public ForbiddenException(string message) : base(message, 403) { }
}