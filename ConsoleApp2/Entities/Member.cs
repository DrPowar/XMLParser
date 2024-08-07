using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace ConsoleApp2.Entities
{
    public sealed record Member(
        uint Id, 
        string? Name, 
        DateTime MembershipDate, 
        List<BorrowedBook> BorrowedBooks)
    {
        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Membership Date: {MembershipDate:yyyy-MM-dd}, Borrowed Books Count: {BorrowedBooks.Count}";
        }
    }
}