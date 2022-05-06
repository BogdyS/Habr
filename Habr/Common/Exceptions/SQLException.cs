namespace Habr.Common.Exceptions;

public class SQLException : Exception
{
    public SQLException(string message) : base(message) { }
}
