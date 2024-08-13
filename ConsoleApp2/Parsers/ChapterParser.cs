using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.XMLUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Parsers
{
    internal class ChapterParser
    {
        public static Chapter ParseChapter(XmlNode node)
        {
            uint number = uint.Parse(node.Attributes[ChapterConst.Number]);
            string title = null;
            string content = null;

            foreach (XmlNode child in node.Children)
            {
                switch (child.Name)
                {
                    case ChapterConst.Title:
                        title = child.InnerText;
                        break;
                    case ChapterConst.Content:
                        content = child.InnerText;
                        break;
                }
            }

            return new Chapter(number, title, content);
        }
    }
}
