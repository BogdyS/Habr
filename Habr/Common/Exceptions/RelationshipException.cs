using Habr.Common.Exceptions.Base;

namespace Habr.Common.Exceptions;

public class RelationshipException : BaseException
{
    public RelationshipException(string message) : base(message, 400) { }
}