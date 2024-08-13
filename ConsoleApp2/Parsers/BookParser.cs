using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.XMLUtils.Models;

namespace ConsoleApp2.Parsers
{
    internal static class BookParser
    {
        public static Book ParseBook(XmlNode node)
        {
            var (id, genre) = ParseBookAttributes(node.Attributes);
            var (title, author, publicationDate, chapters) = ParseChildNodes(node.Children);
            return new Book(id, title, genre, author, publicationDate, chapters);
        }

        private static (uint id, string genre) ParseBookAttributes(Dictionary<string, string> attributes)
        {
            uint id = 0;
            string genre = string.Empty;

            foreach (KeyValuePair<string, string> attribute in attributes)
            {
                if (attribute.Key == BookConst.Id)
                    id = uint.Parse(attribute.Value);
                else if (attribute.Key == BookConst.Genre)
                    genre = attribute.Value;
            }

            return (id, genre);
        }

        private static (string title, string author, uint publicationDate, List<Chapter> chapters) ParseChildNodes(List<XmlNode> children)
        {
            string title = string.Empty;
            string author = string.Empty;
            uint publicationDate = 0;
            List<Chapter> chapters = new List<Chapter>();

            foreach (XmlNode child in children)
            {
                switch (child.Name)
                {
                    case BookConst.Title:
                        title = child.InnerText;
                        break;
                    case BookConst.Author:
                        author = child.InnerText;
                        break;
                    case BookConst.PublicationDate:
                        publicationDate = uint.Parse(child.InnerText);
                        break;
                    case BookConst.Chapters:
                        chapters.AddRange(child.Children.Select(c => ChapterParser.ParseChapter(c)));
                        break;
                }
            }

            return (title, author, publicationDate, chapters);
        }
    }
}
