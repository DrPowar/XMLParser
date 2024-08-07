using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace ConsoleApp2.Entities
{
    public sealed record Member
    {
        public uint Id { get; set; }

        public string? Name { get; set; }

        public DateTime MembershipDate { get; set; }

        public List<BorrowedBook> BorrowedBooks { get; set; } = new List<BorrowedBook>();

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Membership Date: {MembershipDate:yyyy-MM-dd}, Borrowed Books Count: {BorrowedBooks.Count}";
        }
    }
}