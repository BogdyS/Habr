namespace Habr.DataAccess
{

    public class SQLException : Exception
    {
        public SQLException(string message) : base(message) {}
    }
}