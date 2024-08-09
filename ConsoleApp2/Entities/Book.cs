using BenchmarkDotNet.Attributes;
using System.Collections.Immutable;

namespace ConsoleApp2.Entities
{
    public sealed record Book(
        uint Id, 
        string? Title, 
        string? Genre, 
        string? Author, 
        uint PublicationDate, 
        List<Chapter> Chapters)
    {
        public override string ToString()
        {
            return $"Id: {Id}, Title: {Title}, Genre: {Genre}, Author: {Author}, Publication Date: {PublicationDate}, Chapters Count: {Chapters.Count}";
        }
    }
}