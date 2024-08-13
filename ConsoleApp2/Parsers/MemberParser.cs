using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.XMLUtils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Parsers
{
    internal static class MemberParser
    {
        public static Member ParseMember(XmlNode memberNode)
        {
            uint id = ParseMemberId(memberNode);
            string name = string.Empty;
            DateTime membershipDate = DateTime.MinValue;
            List<BorrowedBook> borrowedBooks = new List<BorrowedBook>();

            foreach (XmlNode child in memberNode.Children)
            {
                name = ExtractAttribute(child, MemberConst.Name, name);
                membershipDate = ExtractDateAttribute(child, MemberConst.MembershipDate, membershipDate);
                borrowedBooks = ExtractBorrowedBooks(child, MemberConst.BooksBorrowed, borrowedBooks);
            }

            return new Member(id, name, membershipDate, borrowedBooks);
        }

        private static uint ParseMemberId(XmlNode memberNode)
        {
            return uint.Parse(memberNode.Attributes[MemberConst.Id]);
        }

        private static string ExtractAttribute(XmlNode node, string attributeName, string defaultValue)
        {
            return node.Name == attributeName ? node.InnerText : defaultValue;
        }

        private static DateTime ExtractDateAttribute(XmlNode node, string attributeName, DateTime defaultValue)
        {
            return node.Name == attributeName ? DateTime.Parse(node.InnerText) : defaultValue;
        }

        private static List<BorrowedBook> ExtractBorrowedBooks(XmlNode node, string attributeName, List<BorrowedBook> defaultValue)
        {
            if (node.Name == attributeName)
            {
                defaultValue.AddRange(node.Children.Select(b => BorrowedBookParser.ParseBorrowedBook(b)));
            }
            return defaultValue;
        }
    }
}
