using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Microsoft.Maui.Storage;

namespace StudentPerformanceAnalyzer.Services
{
    public class XmlDataService : IXmlDataService
    {
        private const string XmlFileName = "students.xml";
        private const string XslFileName = "students.xsl";

        public async Task<Stream> GetXmlStreamAsync()
        {
            return await FileSystem.OpenAppPackageFileAsync(XmlFileName);
        }

        public async Task<List<string>> GetUniqueValuesAsync(string attributeName)
        {
            using var stream = await GetXmlStreamAsync();
            var doc = XDocument.Load(stream);

            return doc.Descendants("Student")
                      .Select(x => (string?)x.Attribute(attributeName))
                      .Where(x => !string.IsNullOrEmpty(x))
                      .Distinct()
                      .OrderBy(x => x)
                      .ToList()!;
        }

        public async Task TransformToHtmlAsync(string outputFilePath)
        {
            using var xmlStream = await FileSystem.OpenAppPackageFileAsync(XmlFileName);
            using var xslStream = await FileSystem.OpenAppPackageFileAsync(XslFileName);

            using var xmlReader = XmlReader.Create(xmlStream);
            using var xslReader = XmlReader.Create(xslStream);

            var transform = new XslCompiledTransform();
            transform.Load(xslReader);

            using var writer = XmlWriter.Create(outputFilePath, transform.OutputSettings ?? new XmlWriterSettings { Indent = true });
            transform.Transform(xmlReader, writer);
        }
    }
}