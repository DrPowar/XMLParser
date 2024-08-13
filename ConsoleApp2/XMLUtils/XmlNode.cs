namespace ConsoleApp2.XMLUtils
{
    public sealed class XmlNode
    {
        public required string Name { get; init; }
        public string? InnerText { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new();
        public List<XmlNode> Children { get; set; } = [];
    }
}
