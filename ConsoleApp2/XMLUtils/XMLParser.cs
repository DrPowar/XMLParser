using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.Exceptions;
using ConsoleApp2.Parsers;
using System;
using System.Text;
using static ConsoleApp2.XMLUtils.XMLStringHelper;

namespace ConsoleApp2.XMLUtils
{
    public class XMLParser
    {
        public List<Library> ParseXML(string xml)
        {
            XMLValidator xMLValidator = new XMLValidator();
            ValidationResult validation = xMLValidator.IsValid(xml);

            List<Range> invalidNodesRange = xMLValidator.Errors.Keys.ToList();

            xml = RemoveInvalidNodes(xml, invalidNodesRange); 

            if (validation.Result == ValidationResultType.CriticalFailure)
            {
                throw new InvalidXMLException("Critical failure: " + validation.ValidationMessage);
            }
            else
            {
                return ParseLibrary(xml);
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
                foreach (XmlNode child in node.Children)
                {
                    List<Book> books = ParseNodes(child, LibraryConst.Books, BookParser.ParseBook);

                    List<Member> members = ParseNodes(child, LibraryConst.Members, MemberParser.ParseMember);

                    libs.Add(new Library(books, members));
                }
            }

            return libs;
        }

        private List<T> ParseNodes<T>(XmlNode xmlNode, string expectedName, Func<XmlNode, T> parseFunc)
        {
            if (xmlNode.Name == expectedName)
            {
                return xmlNode.Children.Select(parseFunc).ToList();
            }
            else
            {
                return new List<T>();
            }
        }

        private XmlNode GetNodeTree(string xml, ref int index)
        {
            return ParseElement(xml, ref index);
        }

        private XmlNode ParseElement(string xml, ref int index)
        {
            SkipWhiteSpaces(xml, ref index);

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagOnpeningBracket);
            string tagName = ParseTagName(xml, ref index);

            XmlNode node = new XmlNode { Name = tagName };

            HandleAttribute(xml, ref index, node);

            if (IsTagClosed(xml[index]))
            {
                SkipSymbols(xml, ref index, new List<XMLSymbols> { XMLSymbols.XmlTagCloseBracket, XMLSymbols.XmlSelfClosingSlash });
                return node;
            }

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagCloseBracket);
            HandleInnerContext(xml, ref index, node);

            SkipSymbols(xml, ref index, new List<XMLSymbols> { XMLSymbols.XmlTagOnpeningBracket, XMLSymbols.XmlSelfClosingSlash });
            SkipWhiteSpaces(xml, ref index);
            string closingTagName = ParseTagName(xml, ref index);

            SkipSymbol(xml, ref index, XMLSymbols.XmlTagCloseBracket);
            SkipSymbols(xml, ref index, new List<XMLSymbols> { XMLSymbols.NextLineSymbol, XMLSymbols.CarriageReturn });

            return node;
        }

        private string ParseTagName(string xml, ref int index)
        {
            int start = index;

            while (index < xml.Length && IsXMLTagSymbolValid(xml[index]))
            {
                index++;    
            }
            return xml.Substring(start, index - start);
        }

        private (string, string) ParseAttribute(string xml, ref int index)
        {
            string name = ParseTagName(xml, ref index);
            SkipWhiteSpaces(xml, ref index);

            index++;
            SkipWhiteSpaces(xml, ref index);

            index++;
            int start = index;

            while (index < xml.Length && !IsSymbol(xml, index, XMLSymbols.AttributeValueDelimiterSign))
            {
                index++;
            }

            string value = xml.Substring(start, index - start);
            index++;

            return (name, value);
        }

        private string ParseTextContent(string xml, ref int index)
        {
            int start = index;

            while (index < xml.Length && !IsSymbol(xml, index, XMLSymbols.XmlTagOnpeningBracket))
            {
                index++;
            }

            return xml.Substring(start, index - start).Trim();
        }

        private void HandleInnerContext(string xml, ref int index, XmlNode node)
        {
            while (!IsSymbol(xml, index, XMLSymbols.XmlTagOnpeningBracket) || !IsSymbol(xml, index + 1, XMLSymbols.XmlSelfClosingSlash))
            {
                if (xml[index] == (char)XMLSymbols.XmlTagOnpeningBracket)
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
            SkipWhiteSpaces(xml, ref index);

            while (!IsEndOfTag(xml[index]))
            {
                SkipWhiteSpaces(xml, ref index);
                if (!IsEndOfTag(xml[index]))
                {
                    var (attrName, attrValue) = ParseAttribute(xml, ref index);
                    node.Attributes[attrName] = attrValue;
                }
            }
        }
    }
}