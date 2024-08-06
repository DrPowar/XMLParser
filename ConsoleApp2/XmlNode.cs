namespace ConsoleApp2
{
    public sealed class XmlNode
    {
        public string Name { get; set; }
        public string InnerText { get; set; }
        public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
        public List<XmlNode> Children { get; set; } = new List<XmlNode>();
    }
}
