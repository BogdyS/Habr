using Habr.Common.DTO;

namespace Habr.ConsoleApp.UI
{
    public class ConsolePostUI
    {
        static public void OutputPostList(IEnumerable<PostListDTO> list)
        {
            Console.Write('|' + "Title");
            Console.SetCursorPosition(201, Console.CursorTop);
            Console.Write("| " + "UserEmail");
            Console.SetCursorPosition(402, Console.CursorTop);
            Console.Write("| " + "Created");
            Console.SetCursorPosition(430, Console.CursorTop);
            Console.WriteLine("|");
            foreach (var post in list.OrderBy(p=>p.Created))
            {
                Console.Write('|' + post.Title);
                Console.SetCursorPosition(201, Console.CursorTop);
                Console.Write("| " + post.UserEmail);
                Console.SetCursorPosition(402, Console.CursorTop);
                Console.Write("| " + post.Created);
                Console.SetCursorPosition(430, Console.CursorTop);
                Console.WriteLine("|");
            }
        }

        static public void OutputDraftPostList(IEnumerable<PostDraftDTO> list)
        {
            Console.Write('|' + "Title");
            Console.SetCursorPosition(201, Console.CursorTop);
            Console.Write("| " + "Created");
            Console.SetCursorPosition(231, Console.CursorTop);
            Console.Write("| " + "Updated");
            Console.SetCursorPosition(261, Console.CursorTop);
            Console.WriteLine("|");
            foreach (var post in list.OrderBy(p => p.Updated))
            {
                Console.Write('|' + post.Title);
                Console.SetCursorPosition(201, Console.CursorTop);
                Console.Write("| " + post.Created);
                Console.SetCursorPosition(231, Console.CursorTop);
                Console.Write("| " + post.Updated);
                Console.SetCursorPosition(261, Console.CursorTop);
                Console.WriteLine("|");
            }
        }
    }
}