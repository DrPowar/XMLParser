using BenchmarkDotNet.Attributes;
using ConsoleApp2.Constants;
using ConsoleApp2.Entities;
using ConsoleApp2.XML.Utils;
using ConsoleApp2.XMLUtils;
using ConsoleApp2.XMLUtils.Models;
using System.Text;

namespace ConsoleApp2
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class Benchmark
    {
        private string? xml;

        private XMLParser? xmlParser;

        private XMLValidator? xmlValidator;

        [GlobalSetup]
        public void GlobalSetup()
        {
            xml = GetXMLFile();
            xmlParser = new XMLParser();
            xmlValidator = new XMLValidator();
        }

        [Benchmark]
        public string GetXMLFile()
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader sr = new StreamReader(FilePathConst.ValidFilePath))
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
        public ValidationResult IsXMLValid()
        {
            return xmlValidator!.IsValid(xml!);
        }

        [Benchmark]
        public List<Library> GetLibraryFromXml()
        {
            return xmlParser!.ParseLibrary(xml!);
        } 
    }
}
