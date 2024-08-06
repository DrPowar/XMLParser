namespace ConsoleApp2
{
    internal sealed class Member
    {
        public uint Id { get; set; }  
        
        public string Name { get; set; }

        public DateTime MembershipDate { get; set; }

        public List<BorrowedBook> BorrowedBooks { get; set; }
    }
}