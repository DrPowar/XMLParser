using BenchmarkDotNet.Running;
using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.Exceptions;
using ConsoleApp2.XML.Utils;
using ConsoleApp2.XMLUtils;
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
            string xml = GetXMLFile(FilePathConst.InvalidFilePath);

            XMLParser parser = new XMLParser();

            List<Library> libs = new List<Library>();

            XMLValidator xMLValidator = new XMLValidator();

            try
            {
                libs = parser.ParseXML(xml);
            }
            catch(InvalidXMLException e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }

            //Показує всі дані які є в бібліотеці
            foreach(Library library in libs)
            {
                library.IntroduceLibrary();
            }

            //Скільки всьо чаптерів в бібліотеці
            GetAllChpaters(libs);

            //Які книги хто позичив
            BorrowedBooks(libs);

            //Книги відсортовані за жанром
            SortedByGenreBooks(libs);
        }

        private static string GetXMLFile(string file)
        {
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

            return s;
        }

        private static void GetAllChpaters(List<Library> libraries)
        {
            foreach (Library library in libraries)
            {
                int totalChapters = library.Books.SelectMany(b => b.Chapters).Count();
                Console.WriteLine($"\n\nAll books in the library have a total of {totalChapters} chapters.");
            }
        }

        private static void BorrowedBooks(List<Library> libraries)
        {
            foreach (Library library in libraries)
            {
                foreach (Member member in library.Members)
                {
                    Console.WriteLine($"\n\nMember {member.Id} borrowed the following books:");

                    List<uint> borrowedBookIds = member.BorrowedBooks.Select(b => b.Id).ToList();

                    List<Book> borrowedBooks = library.Books.Where(b => borrowedBookIds.Contains((uint)b.Id)).ToList();

                    foreach (Book book in borrowedBooks)
                    {
                        Console.WriteLine($"- {book.Title}");
                    }
                }
            }
        }

        private static void SortedByGenreBooks(List<Library> libraries)
        {
            foreach (Library library in libraries)
            {
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
}