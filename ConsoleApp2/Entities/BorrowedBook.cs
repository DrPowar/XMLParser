using BenchmarkDotNet.Attributes;

namespace ConsoleApp2.Entities
{
    public sealed record BorrowedBook(
        uint Id, 
        DateTime DueDate)
    {
        public override string ToString()
        {
            return $"Book ID: {Id}, Due Date: {DueDate:yyyy-MM-dd}";
        }
    }
}