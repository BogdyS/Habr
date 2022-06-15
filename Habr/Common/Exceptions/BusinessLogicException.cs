using Habr.Common.Exceptions.Base;

namespace Habr.Common.Exceptions;

public class BusinessLogicException : BaseException
{
    public BusinessLogicException(string message) : base(message, 401) { }
}