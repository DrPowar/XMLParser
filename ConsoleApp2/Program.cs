using System.Text;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main()
        {
            string file = "C:\\Users\\User\\Desktop\\LibraryXML_test.txt";
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(file))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
            }

            string s = sb.ToString();
            sb.Clear();

            Library library = XMLParser.ParseLibrary(s);
            library.IntroduceLibrary();
        }
    }
}