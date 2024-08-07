﻿using BenchmarkDotNet.Attributes;

namespace ConsoleApp2
{
    public sealed class Chapter
    {
        public uint Number { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public override string ToString()
        {
            return $"Chapter {Number}: {Title}";
        }
    }
}