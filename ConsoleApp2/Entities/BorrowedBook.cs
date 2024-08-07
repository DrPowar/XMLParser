using BenchmarkDotNet.Attributes;

namespace ConsoleApp2.Entities
{
    public sealed record BorrowedBook
    {
        public uint Id { get; set; }
        public DateTime DueDate { get; set; }

        public override string ToString()
        {
            return $"Book ID: {Id}, Due Date: {DueDate:yyyy-MM-dd}";
        }
    }
}