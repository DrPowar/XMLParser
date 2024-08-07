using BenchmarkDotNet.Attributes;

namespace ConsoleApp2.Entities
{
    public sealed record Chapter(
        uint Number, 
        string? Title, 
        string? Content)
    {
        public override string ToString()
        {
            return $"Chapter {Number}: {Title}";
        }
    }
}