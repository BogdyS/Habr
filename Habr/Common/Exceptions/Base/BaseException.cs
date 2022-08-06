namespace Habr.Common.Exceptions.Base;

public class BaseException : Exception
{
    public BaseException(string message, int code) : base(message)
    {
        Code = code;
    }
    public int Code { get; }
}