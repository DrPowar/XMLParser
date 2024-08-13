using ConsoleApp2.Constants;
using ConsoleApp2.XMLUtils;
using ConsoleApp2.XMLUtils.Models;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleApp2.XML.Utils
{
    internal static class XMLStringHelper
    {
        public static int GetLineStartIndex(string xml, int index)
        {
            int lineStart;
            if (index > 0)
            {
                lineStart = xml.LastIndexOf((char)XMLSymbols.NextLineSymbol, index - 1);
            }
            else
            {
                lineStart = xml.LastIndexOf((char)XMLSymbols.NextLineSymbol, index);
            }
            return lineStart == -1 ? 0 : lineStart + 1;
        }

        public static int GetLineEndIndex(string xml, int index)
        {
            int lineEnd;
            if (index > 0)
            {
                lineEnd = xml.IndexOf((char)XMLSymbols.NextLineSymbol, index - 1);
            }
            else
            {
                lineEnd = xml.IndexOf((char)XMLSymbols.NextLineSymbol, index);
            }
            return lineEnd == -1 ? xml.Length : lineEnd;
        }

        public static bool IsEndOfTag(char symbol) =>
            symbol is (char)XMLSymbols.XmlTagCloseBracket or (char)XMLSymbols.XmlSelfClosingSlash;

        public static bool IsTagClosed(char symbol) =>
            symbol == (char)XMLSymbols.XmlSelfClosingSlash;

        public static string GetFullLine(string xml, int index)
        {
            int lineStart = GetLineStartIndex(xml, index);
            int lineEnd = GetLineEndIndex(xml, index);
            string line = string.Empty;
            try
            {
                line = xml.Substring(lineStart, lineEnd - lineStart);
            }
            catch (ArgumentOutOfRangeException e)
            {
                return string.Empty;
            }

            return line;
        }

        public static bool IsXMLTagSymbolValid(char symbol) =>
            char.IsLetterOrDigit(symbol) || symbol == (char)XMLSymbols.Colon || symbol == (char)XMLSymbols.Underscore;

        public static void SkipWhiteSpaces(string xml, ref int index)
        {
            while (xml[index] == (char)XMLSymbols.Whitespace)
            {
                index++;
            }
        }

        public static void SkipSymbol(string xml, ref int index, XMLSymbols symbol)
        {
            if (xml[index] == (char)symbol)
            {
                index++;
            }
        }

        public static void SkipSymbols(string xml, ref int index, List<XMLSymbols> symbols)
        {
            for (int i = 0; i < symbols.Count; i++)
            {
                if (symbols.Contains((XMLSymbols)xml[index]))
                {
                    index++;
                }
            }
        }

        public static string RemoveInvalidNodes(string xml, Dictionary<NodeRange, List<string>> errors)
        {
            List<NodeRange> invalidNodesRange = errors.Keys.ToList();
            StringBuilder sb = new(xml);

            invalidNodesRange.Sort((a, b) => b.End.CompareTo(a.End));

            foreach (NodeRange range in invalidNodesRange)
            {
                sb.Remove((int)range.Start, (int)(range.End - range.Start));
            }

            return sb.ToString();
        }


        public static bool IsSymbol(string xml, int index, XMLSymbols symbol)
        {
            if (index < xml.Length)
            {
                return xml[index] == (char)symbol;
            }
            else
            {
                return false;
            }
        }
    }
}
