namespace Habr.DataAccess.Exceptions
{
    public class AccessException : Exception
    {
        public AccessException(string message) : base(message) {}
    }
}
