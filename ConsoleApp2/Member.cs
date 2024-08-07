using BenchmarkDotNet.Attributes;

namespace ConsoleApp2
{
    public sealed class Member
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