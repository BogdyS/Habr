namespace Habr.Common.Exceptions;

public class InputException : Exception
{
    public InputException(string message, string? value,int code) : base(message)
    {
        Value = value;
        Code = code;
    }
    public string? Value { get; }
    public int Code { get; }
}