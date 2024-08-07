using BenchmarkDotNet.Attributes;
using System.Text;

namespace ConsoleApp2
{
    [MemoryDiagnoser]
    [ShortRunJob]
    public class Benchmark
    {
        private string xml;

        private XMLParser xmlParser;

        [GlobalSetup]
        public void GlobalSetup()
        {
            xml = GetXMLFile();
            xmlParser = new XMLParser();
        }

        [Benchmark]
        public string GetXMLFile()
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
            return s;
        }

        [Benchmark]
        public Library GetLibraryFromXml()
        {
            return xmlParser.ParseLibrary(xml);
        } 
    }
}
