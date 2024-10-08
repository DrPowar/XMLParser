﻿using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace ConsoleApp2.Entities
{
    public sealed record Library(List<Book> Books, List<Member> Members)
    {
        public void IntroduceLibrary()
        {
            Console.WriteLine("Library:");
            Console.WriteLine("  Books:");
            foreach (Book book in Books)
            {
                Console.WriteLine("    " + book);
                Console.WriteLine("      Chapters:");
                foreach (Chapter chapter in book.Chapters)
                {
                    Console.WriteLine("        " + chapter);
                }
            }
            Console.WriteLine("  Members:");
            foreach (Member member in Members)
            {
                Console.WriteLine("    " + member);
                Console.WriteLine("      Books:");
                foreach (BorrowedBook book in member.BorrowedBooks)
                {
                    Console.WriteLine("        " + book);
                }
            }
        }

    }
}
