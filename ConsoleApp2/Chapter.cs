namespace ConsoleApp2
{
    internal sealed class Chapter
    {
        public uint Number { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public override string ToString()
        {
            return $"Chapter {Number}: {Title}";
        }
    }
}