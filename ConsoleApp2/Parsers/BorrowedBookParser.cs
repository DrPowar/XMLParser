using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.XMLUtils;

namespace ConsoleApp2.Parsers
{
    internal static class BorrowedBookParser
    {
        public static BorrowedBook ParseBorrowedBook(XmlNode borrowedBookNode)
        {
            uint id = uint.Parse(borrowedBookNode.Attributes[BorrowedBookConst.Id]);
            DateTime dueDate = DateTime.Parse(borrowedBookNode.Attributes[BorrowedBookConst.DueDate]);

            return new BorrowedBook(id, dueDate);
        }
    }
}
