namespace Habr.Common.Exceptions;

public class InvalidDataException : InputException
{
    public InvalidDataException(string message, string value) : base(message, value, 400) {}
}