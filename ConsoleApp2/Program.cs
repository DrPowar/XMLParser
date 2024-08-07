using BenchmarkDotNet.Running;
using System.Reflection;
using System.Text;

BenchmarkSwitcher
    .FromAssembly(Assembly.GetExecutingAssembly())
    .Run(args);

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main()
        {
            string file = "C:\\Users\\User\\Desktop\\LibraryXML.txt";
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }
            string s = sb.ToString();
            sb.Clear();

            XMLParser parser = new XMLParser();

            //Показує всі дані які є в бібліотеці
            Library library = parser.ParseLibrary(s);

            library.IntroduceLibrary();

            //Скільки всьо чаптерів в бібліотеці
            var totalChapters = library.Books.SelectMany(b => b.Chapters).Count();
            Console.WriteLine($"\n\nAll books in the library have a total of {totalChapters} chapters.");

            //Які книги хто позичив
            foreach (var member in library.Members)
            {
                Console.WriteLine($"\n\nMember {member.Id} borrowed the following books:");

                var borrowedBookIds = member.BorrowedBooks.Select(b => b.Id).ToList();

                var borrowedBooks = library.Books.Where(b => borrowedBookIds.Contains((uint)b.Id)).ToList();

                foreach (var book in borrowedBooks)
                {
                    Console.WriteLine($"- {book.Title}");
                }
            }

            //Книги відсортовані за жанром
            Console.WriteLine("\n\nBooks sorted by genre:");
            var sortedBooks = library.Books
            .OrderBy(b => b.Genre)
            .ToList();
            foreach (var book in sortedBooks)
            {
                Console.WriteLine(book.ToString());
            }
        }
    }
}