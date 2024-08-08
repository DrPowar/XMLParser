using BenchmarkDotNet.Running;
using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.Exceptions;
using System.Reflection;
using System.Text;

//BenchmarkSwitcher
//    .FromAssembly(Assembly.GetExecutingAssembly())
//    .Run(args);

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(FilePathConst.ValidFilePath))
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

            Library library = new Library(new List<Book>(), new List<Member>());


            XMLValidator xMLValidator = new XMLValidator();

            try
            {
                library = parser.ParseLibrary(s);
            }
            catch(InvalidXMLException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            //Показує всі дані які є в бібліотеці
            library.IntroduceLibrary();

            //Скільки всьо чаптерів в бібліотеці
            int totalChapters = library.Books.SelectMany(b => b.Chapters).Count();
            Console.WriteLine($"\n\nAll books in the library have a total of {totalChapters} chapters.");

            //Які книги хто позичив
            foreach (Member member in library.Members)
            {
                Console.WriteLine($"\n\nMember {member.Id} borrowed the following books:");

                List<uint> borrowedBookIds = member.BorrowedBooks.Select(b => b.Id).ToList();

                List<Book> borrowedBooks = library.Books.Where(b => borrowedBookIds.Contains((uint)b.Id)).ToList();

                foreach (var book in borrowedBooks)
                {
                    Console.WriteLine($"- {book.Title}");
                }
            }

            //Книги відсортовані за жанром
            Console.WriteLine("\n\nBooks sorted by genre:");
            List<Book> sortedBooks = library.Books
            .OrderBy(b => b.Genre)
            .ToList();
            foreach (Book book in sortedBooks)
            {
                Console.WriteLine(book.ToString());
            }
        }
    }
}