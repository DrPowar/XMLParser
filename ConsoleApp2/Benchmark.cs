using BenchmarkDotNet.Attributes;
using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using System.Text;

namespace ConsoleApp2
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class Benchmark
    {
        private string? xml;

        private XMLParser? xmlParser;

        [GlobalSetup]
        public void GlobalSetup()
        {
            xml = GetXMLFile();
            xmlParser = new XMLParser();
        }

        [Benchmark]
        public string GetXMLFile()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(FilePathConst.FilePath))
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
            return xmlParser!.ParseLibrary(xml!);
        } 
    }
}
