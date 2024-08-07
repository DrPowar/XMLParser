using BenchmarkDotNet.Attributes;

namespace ConsoleApp2
{
    public sealed class BorrowedBook
    {
        public int Id { get; set; }
        public DateTime DueDate { get; set; }

        public override string ToString()
        {
            return $"Book ID: {Id}, Due Date: {DueDate:yyyy-MM-dd}";
        }
    }
}