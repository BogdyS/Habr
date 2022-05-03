using Habr.BusinessLogic.Servises;
using Habr.DataAccess.Entities;

namespace Habr.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            UserService service = new UserService();
            service.LoginAsync("fsfsf", "fsaf");
        }
    }
}

