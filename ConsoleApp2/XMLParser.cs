using BenchmarkDotNet.Attributes;

namespace ConsoleApp2
{
    public class XMLParser
    {
        public Library ParseLibrary(string xml)
        {
            XmlNode libraryNode = GetNodeTree(xml);
            Library library = new Library();

            foreach (var child in libraryNode.Children)
            {
                if (child.Name == "Books")
                {
                    foreach (var book in child.Children)
                    {
                        library.Books.Add(ParseBook(book));
                    }
                }
                else if (child.Name == "Members")
                {
                    foreach (var member in child.Children)
                    {
                        library.Members.Add(ParseMember(member));
                    }
                }
            }
            return library;
        }

        public Book ParseBook(XmlNode node)
        {
            Book book = new Book();

            foreach(var attribute in node.Attributes)
            {
                if(attribute.Key == "id")
                    book.Id = uint.Parse(attribute.Value);

                else if(attribute.Key == "genre")
                    book.Genre = attribute.Value;
            }

            foreach(var child in node.Children)
            {
                switch(child.Name)
                {
                    case "Title":
                        book.Title = child.InnerText;
                        break;
                    case "Author":
                        book.Author = child.InnerText;
                        break;
                    case "PublicationDate":
                        book.PublicationDate = uint.Parse(child.InnerText);
                        break;
                    case "Chapters":
                        foreach (var chapterNode in child.Children)
                        {
                            var chapter = ParseChapter(chapterNode);
                            book.Chapters.Add(chapter);
                        }
                        break;
                }
            }
            return book;
        }

        public Chapter ParseChapter(XmlNode node)
        {
            var chapter = new Chapter
            {
                Number = uint.Parse(node.Attributes["number"])
            };

            foreach (var child in node.Children)
            {
                if (child.Name == "Title")
                    chapter.Title = child.InnerText;

                else if (child.Name == "Content")
                    chapter.Content = child.InnerText;
            }
            return chapter;
        }

        public Member ParseMember(XmlNode memberNode)
        {
            var member = new Member
            {
                Id = uint.Parse(memberNode.Attributes["id"])
            };

            foreach (var child in memberNode.Children)
            {
                if (child.Name == "Name")
                    member.Name = child.InnerText;

                else if (child.Name == "MembershipDate")
                    member.MembershipDate = DateTime.Parse(child.InnerText);

                else if (child.Name == "BooksBorrowed")
                {
                    foreach (var borrowedBookNode in child.Children)
                    {
                        member.BorrowedBooks.Add(ParseBorrowedBook(borrowedBookNode));
                    }
                }
            }
            return member;
        }

        public BorrowedBook ParseBorrowedBook(XmlNode borrowedBookNode)
        {
            BorrowedBook book = new BorrowedBook()
            {
                Id = int.Parse(borrowedBookNode.Attributes["id"]),
                DueDate = DateTime.Parse(borrowedBookNode.Attributes["dueDate"])
            };

            return book;
        }

        public XmlNode GetNodeTree(string xml)
        {
            int index = 0;
            return ParseElement(xml, ref index);
        }

        public XmlNode ParseElement(string xml, ref int index)
        {
            SkipWhitespace(xml, ref index);

            if (xml[index] != '<')
                throw new Exception("Expected '<'");

            index++;
            SkipWhitespace(xml, ref index);
            string tagName = ParseTagName(xml, ref index);

            XmlNode node = new XmlNode { Name = tagName };

            while (xml[index] != '>' && xml[index] != '/')
            {
                SkipWhitespace(xml, ref index);
                if (xml[index] != '>' && xml[index] != '/')
                {
                    var (attrName, attrValue) = ParseAttribute(xml, ref index);
                    node.Attributes[attrName] = attrValue;
                }
            }

            if (xml[index] == '/')
            {
                index += 2; //Skip />
                return node;
            }

            index++;

            while (xml[index] != '<' || xml[index + 1] != '/')
            {
                if (xml[index] == '<')
                {
                    var childNode = ParseElement(xml, ref index);
                    node.Children.Add(childNode);
                }
                else
                {
                    node.InnerText += ParseTextContent(xml, ref index);
                }
            }
            index += 2;
            SkipWhitespace(xml, ref index);
            string closingTagName = ParseTagName(xml, ref index);

            if (closingTagName != tagName)
                throw new Exception("Mismatched closing tag");

            index++;

            return node;
        }

        public string ParseTagName(string xml, ref int index)
        {
            int start = index;
            while (index < xml.Length && (char.IsLetterOrDigit(xml[index]) || xml[index] == ':' || xml[index] == '_'))
            {
                index++;
            }
            return xml.Substring(start, index - start);
        }

        public (string, string) ParseAttribute(string xml, ref int index)
        {
            string name = ParseTagName(xml, ref index);

            SkipWhitespace(xml, ref index);

            if (xml[index] != '=')
                throw new Exception("Expected '=' in attribute");

            index++;
            SkipWhitespace(xml, ref index);

            if (xml[index] != '"')
                throw new Exception("Expected '\"' in attribute value");

            index++;
            int start = index;

            while (index < xml.Length && xml[index] != '"')
            {
                index++;
            }

            string value = xml.Substring(start, index - start);
            index++;

            return (name, value);
        }

        public string ParseTextContent(string xml, ref int index)
        {
            int start = index;
            while (index < xml.Length && xml[index] != '<')
            {
                index++;
            }
            return xml.Substring(start, index - start).Trim();
        }

        public void SkipWhitespace(string xml, ref int index)
        {
            while (index < xml.Length && char.IsWhiteSpace(xml[index]))
            {
                index++;
            }
        }
    }
}

//< Library >
//  < Books >
//    < Book id = "1" genre = "fiction" >
//      < Title > The Great Gatsby</Title>
//      <Author>F.Scott Fitzgerald</Author>
//      <PublicationDate>1925</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "2" genre= "fiction" >
//      < Title > To Kill a Mockingbird</Title>
//      <Author>Harper Lee</Author>
//      <PublicationDate>1960</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "3" genre= "fiction" >
//      < Title > 1984 </ Title >
//      < Author > George Orwell</Author>
//      <PublicationDate>1949</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//        <Chapter number = "3" >
//          < Title > Chapter Three</Title>
//          <Content>This is the content of chapter three...</Content>
//        </Chapter>
//        <Chapter number = "4" >
//          < Title > Chapter Four</Title>
//          <Content>This is the content of chapter four...</Content>
//        </Chapter>
//        <Chapter number = "5" >
//          < Title > Chapter Five</Title>
//          <Content>This is the content of chapter five...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "4" genre= "fantasy" >
//      < Title > Harry Potter and the Sorcerer's Stone</Title>
//      <Author>J.K.Rowling</Author>
//      <PublicationDate>1997</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//        <Chapter number = "3" >
//          < Title > Chapter Three</Title>
//          <Content>This is the content of chapter three...</Content>
//        </Chapter>
//        <Chapter number = "4" >
//          < Title > Chapter Four</Title>
//          <Content>This is the content of chapter four...</Content>
//        </Chapter>
//        <Chapter number = "5" >
//          < Title > Chapter Five</Title>
//          <Content>This is the content of chapter five...</Content>
//        </Chapter>
//        <Chapter number = "6" >
//          < Title > Chapter Six</Title>
//          <Content>This is the content of chapter six...</Content>
//        </Chapter>
//        <Chapter number = "7" >
//          < Title > Chapter Seven</Title>
//          <Content>This is the content of chapter seven...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//    <Book id = "5" genre= "science fiction" >
//      < Title > Dune </ Title >
//      < Author > Frank Herbert</Author>
//      <PublicationDate>1965</PublicationDate>
//      <Chapters>
//        <Chapter number = "1" >
//          < Title > Chapter One</Title>
//          <Content>This is the content of chapter one...</Content>
//        </Chapter>
//        <Chapter number = "2" >
//          < Title > Chapter Two</Title>
//          <Content>This is the content of chapter two...</Content>
//        </Chapter>
//        <Chapter number = "3" >
//          < Title > Chapter Three</Title>
//          <Content>This is the content of chapter three...</Content>
//        </Chapter>
//      </Chapters>
//    </Book>
//  </Books>
//  <Members>
//    <Member id = "1001" >
//      < Name > John Doe</Name>
//      <MembershipDate>2021-01-15</MembershipDate>
//      <BooksBorrowed>
//        <Book id = "1" dueDate= "2021-02-15" />
//      </ BooksBorrowed >
//    </ Member >
//  </ Members >
//</ Library >
