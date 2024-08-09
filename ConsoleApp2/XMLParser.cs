using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.Exceptions;

namespace ConsoleApp2
{
    public class XMLParser
    {
        public List<Library> ParseXML(string xml)
        {
            XMLValidator xMLValidator = new XMLValidator();
            ValidationResult validation = xMLValidator.IsValid(xml);

            if(validation.Result)
            {
                return ParseLibrary(xml);
            }
            else
            {
                throw new InvalidXMLException(validation.ValidationMessage);
            }
        }

        public List<Library> ParseLibrary(string xml)
        {
            int index = 0;

            List<XmlNode> nodes = new List<XmlNode>();

            while (index < xml.Length)
            {
                nodes.Add(GetNodeTree(xml, ref index));
            }

            List<Library> libs = new List<Library>();

            foreach (XmlNode node in nodes)
            {
                List<Book> books = new List<Book>();
                List<Member> members = new List<Member>();

                foreach (XmlNode child in node.Children)
                {
                    if (child.Name == LibraryConst.Books)
                        books.AddRange(child.Children.Select(b => ParseBook(b)));

                    else if (child.Name == LibraryConst.Members)
                        members.AddRange(child.Children.Select(m => ParseMember(m)));
                }
                libs.Add(new Library(books, members));
            }

            return libs;
        }

        private Book ParseBook(XmlNode node)
        {
            uint id = 0;
            string genre = string.Empty;
            string title = string.Empty;
            string author = string.Empty;
            uint publicationDate = 0;
            List<Chapter> chapters = new List<Chapter>();

            foreach(KeyValuePair<string, string> attribute in node.Attributes)
            {
                if(attribute.Key == BookConst.Id)
                    id = uint.Parse(attribute.Value);

                else if(attribute.Key == BookConst.Genre)
                    genre = attribute.Value;
            }

            foreach(XmlNode child in node.Children)
            {
                switch(child.Name)
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
                        chapters.AddRange(child.Children.Select(c => ParseChapter(c)));
                        break;
                }
            }
            return new Book(id, title, genre, author, publicationDate, chapters);
        }

        private Chapter ParseChapter(XmlNode node)
        {
            uint number = 0;
            string title = string.Empty;
            string content = string.Empty;

            number = uint.Parse(node.Attributes[ChapterConst.Number]);

            foreach (XmlNode child in node.Children)
            {
                if (child.Name == ChapterConst.Title)
                    title = child.InnerText;

                else if (child.Name == ChapterConst.Content)
                    content = child.InnerText;
            }
            return new Chapter(number, title, content);
        }

        private Member ParseMember(XmlNode memberNode)
        {
            uint id = 0;
            string name = string.Empty;
            DateTime membershipDate = DateTime.MinValue;
            List<BorrowedBook> borrowedBooks = new List<BorrowedBook>();

            id = uint.Parse(memberNode.Attributes[MemberConst.Id]);

            foreach (XmlNode child in memberNode.Children)
            {
                if (child.Name == MemberConst.Name)
                    name = child.InnerText;

                else if (child.Name == MemberConst.MembershipDate)
                    membershipDate = DateTime.Parse(child.InnerText);

                else if (child.Name == MemberConst.BooksBorrowed)
                    borrowedBooks.AddRange(child.Children.Select(b => ParseBorrowedBook(b)));
            }
            return new Member(id, name, membershipDate, borrowedBooks);
        }

        private BorrowedBook ParseBorrowedBook(XmlNode borrowedBookNode)
        {
            uint id = uint.Parse(borrowedBookNode.Attributes[BorrowedBookConst.Id]);
            DateTime dueDate = DateTime.Parse(borrowedBookNode.Attributes[BorrowedBookConst.DueDate]);

            return new BorrowedBook(id, dueDate);
        }

        private XmlNode GetNodeTree(string xml, ref int index)
        {
            return ParseElement(xml, ref index);
        }

        private XmlNode ParseElement(string xml, ref int index)
        {
            SkipWhitespace(xml, ref index);

            SkipOpeningSymbol(xml, ref index);
            string tagName = ParseTagName(xml, ref index);

            XmlNode node = new XmlNode { Name = tagName };

            HandleAttribute(xml, ref index, node);

            if (IsTagClosed(xml[index]))
            {
                SkipAutoClosingSymbols(xml, ref index);
                return node;
            }

            SkipClosingSymbol(xml, ref index);
            HandleInnerContext(xml, ref index, node);

            SkipTagClosingSymbols(xml, ref index);
            SkipWhitespace(xml, ref index);
            string closingTagName = ParseTagName(xml, ref index);

            SkipClosingSymbol(xml, ref index);
            SkipEndOfElementSymbols(xml, ref index);

            return node;
        }

        private void SkipAutoClosingSymbols(string xml, ref int index)
        {
            if (xml[index] == XMLSymbolsConst.XmlSelfClosingSlash && xml[index + 1] == XMLSymbolsConst.XmlTagCloseBracket)
                index += 2;
        }

        private void SkipTagClosingSymbols(string xml, ref int index)
        {
            if (xml[index] == XMLSymbolsConst.XmlTagOnpening && xml[index + 1] == XMLSymbolsConst.XmlSelfClosingSlash)
                index += 2;
        }

        private void SkipEndOfElementSymbols(string xml, ref int index)
        {
            if (xml[index] == XMLSymbolsConst.CarriageReturn && xml[index + 1] == XMLSymbolsConst.NextLineSymbol)
                index += 2;
        }

        private void SkipClosingSymbol(string xml, ref int index)
        {
            if (xml[index] == XMLSymbolsConst.XmlTagCloseBracket)
                index++;
        }

        private void SkipOpeningSymbol(string xml, ref int index)
        {
            if (xml[index] == XMLSymbolsConst.XmlTagOnpening)
                index++;
        }

        private string ParseTagName(string xml, ref int index)
        {
            int start = index;

            while (index < xml.Length && IsXMLSymbolValid(xml[index]))
            {
                index++;
            }
            return xml.Substring(start, index - start);
        }

        private bool IsXMLSymbolValid(char symbol) =>
            (char.IsLetterOrDigit(symbol) || symbol == XMLSymbolsConst.Colon || symbol == XMLSymbolsConst.Underscore);

        private (string, string) ParseAttribute(string xml, ref int index)
        {
            string name = ParseTagName(xml, ref index);
            SkipWhitespace(xml, ref index);

            index++;
            SkipWhitespace(xml, ref index);

            index++;
            int start = index;

            while (index < xml.Length && xml[index] != XMLSymbolsConst.AttributeValueDelimiterSign)
                index++;

            string value = xml.Substring(start, index - start);
            index++;

            return (name, value);
        }

        private string ParseTextContent(string xml, ref int index)
        {
            int start = index;

            while (index < xml.Length && xml[index] != XMLSymbolsConst.XmlTagOnpening)
                index++;

            return xml.Substring(start, index - start).Trim();
        }

        private void SkipWhitespace(string xml, ref int index)
        {
            while (index < xml.Length && char.IsWhiteSpace(xml[index]))
                index++;
        }

        private void HandleInnerContext(string xml, ref int index, XmlNode node)
        {
            while (xml[index] != XMLSymbolsConst.XmlTagOnpening || xml[index + 1] != XMLSymbolsConst.XmlSelfClosingSlash)
            {
                if (xml[index] == XMLSymbolsConst.XmlTagOnpening)
                {
                    HandleChildElement(xml, ref index, node);
                }
                else
                {
                    HandleInnerContent(xml, ref index, node);
                }
            }
        }

        private void HandleInnerContent(string xml, ref int index, XmlNode node) =>
            node.InnerText += ParseTextContent(xml, ref index);

        private void HandleChildElement(string xml, ref int index, XmlNode node)
        {
            XmlNode childNode = ParseElement(xml, ref index);
            node.Children.Add(childNode);
        }

        private void HandleAttribute(string xml, ref int index, XmlNode node)
        {
            SkipWhitespace(xml, ref index);

            while (!IsEndOfTag(xml[index]))
            {
                SkipWhitespace(xml, ref index);
                if (!IsEndOfTag(xml[index]))
                {
                    var (attrName, attrValue) = ParseAttribute(xml, ref index);
                    node.Attributes[attrName] = attrValue;
                }
            }
        }

        private bool IsEndOfTag(char symbol) =>
            symbol == XMLSymbolsConst.XmlTagCloseBracket || symbol == XMLSymbolsConst.XmlSelfClosingSlash ? true : false;

        private bool IsTagClosed(char symbol) =>
            symbol == XMLSymbolsConst.XmlSelfClosingSlash ? true : false;
    }
}