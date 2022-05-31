using Habr.Common.Exceptions.Base;

namespace Habr.Common.Exceptions
{
    public class AccessException : BaseException
    {
        public AccessException(string message) : base(message, 403) {}
    }
}
