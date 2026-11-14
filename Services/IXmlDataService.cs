namespace StudentPerformanceAnalyzer.Services
{
    public interface IXmlDataService
    {
        Task<Stream> GetXmlStreamAsync();

        Task<List<string>> GetUniqueValuesAsync(string attributeName);

        Task TransformToHtmlAsync(string outputFilePath);
    }
}