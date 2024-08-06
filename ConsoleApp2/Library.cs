namespace ConsoleApp2
{
    internal sealed class Library
    {
        public List<Book> Books { get; set; } = new List<Book>();
        public List<Member> Members { get; set; } = new List<Member>();



        public void IntroduceLibrary()
        {
            Console.WriteLine("Library:");
            Console.WriteLine("  Books:");
            foreach (Book book in Books)
            {
                Console.WriteLine("    " + book.ToString());
                Console.WriteLine("      Chapters:");
                foreach (Chapter chapter in book.Chapters)
                {
                    Console.WriteLine("        " + chapter.ToString());
                }
            }
            Console.WriteLine("  Members:");
            foreach (Member member in Members)
            {
                Console.WriteLine("    " + member.ToString());
                Console.WriteLine("      Books:");
                foreach (BorrowedBook book in member.BorrowedBooks)
                {
                    Console.WriteLine("        " + book.ToString());
                }
            }
        }
  
    }
}
